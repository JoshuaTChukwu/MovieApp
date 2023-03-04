using GOSBackend.Data;
using GOSBackend.SqlTables;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using GOSLibraries.GOS_Financial_Identity;
using GOSLibraries.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GOSBackend.Handlers
{
    public class LoginServicecs : ILoginServices
    {
        private readonly ILogger<LoginServicecs> _logger;
        private readonly IConfiguration _configuration;
        private readonly DataBaseContext _dbContext;
        private readonly JwtSettings _jwtSettings;
        public readonly UserManager<Users> _userManager;
        public LoginServicecs(ILogger<LoginServicecs> logger, IConfiguration configuration, DataBaseContext dataBaseContext, JwtSettings jwtSettings, UserManager<Users> userManager)
        {
            _logger = logger;
            _configuration = configuration;
            _dbContext = dataBaseContext;
            _jwtSettings = jwtSettings;
            _userManager = userManager;
        }
        public async Task<AuthenticationResult> ClientLoginAsync(Users user)
        {
            try
            {
                return await EmployeeGenerateAuthenticationResultForUserAsync(user);

            }
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.LogError($"ErrorID : LoginAsync{errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new AuthenticationResult
                {

                    Status = new APIResponseStatus
                    {
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Error occured!! Please try again later",
                            MessageId = errorCode,
                            TechnicalMessage = $"ErrorID : LoginAsync{errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}"
                        }
                    }
                };
                #endregion
            }
        }

        private async Task<AuthenticationResult> EmployeeGenerateAuthenticationResultForUserAsync(Users user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.FullName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("userId", user.Id),
                    new Claim("securedERPuser", "clientAdmin") ,
                    new Claim("loginId", user.UserName) ,
                    new Claim("userType",user.UserType.ToString())
                };
               


                var userClaims = await _userManager.GetClaimsAsync(user);

                claims.AddRange(userClaims);








                var tokenDecriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifeSpan),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };


                var token = tokenHandler.CreateToken(tokenDecriptor);

                var refreshToken = new RefreshToken
                {
                    JwtId = token.Id,
                    UserId = user.Id,
                    CreationDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddSeconds(6),
                };

                try
                {
                    //await _dbContext.RefreshToken.AddAsync(refreshToken);
                    //await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return new AuthenticationResult
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = ex?.InnerException?.Message,
                            }
                        }
                    };
                }

                return new AuthenticationResult
                {
                    Token = tokenHandler.WriteToken(token),
                    RefreshToken = refreshToken.Token,
                    Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() }
                };
            }
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.LogError($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new AuthenticationResult
                {

                    Status = new APIResponseStatus
                    {
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Error occured!! Please try again later",
                            MessageId = errorCode,
                            TechnicalMessage = $"ErrorID :{errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}"
                        }
                    }
                };
                #endregion
            }

        }
    }

    public interface ILoginServices
    {
        Task<AuthenticationResult> ClientLoginAsync(Users user);
    }
}
