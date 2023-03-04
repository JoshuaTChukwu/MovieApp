using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MovieApp.Configurations;
using MovieApp.Data;
using MovieApp.Helpers;
using MovieApp.SqlTables;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static MovieApp.Contracts.Common.AuxillaryObjs;

namespace GOSBackend.Handlers
{
    public class LoginServicecs : ILoginServices
    {
        private readonly ILogger<LoginServicecs> _logger;
        private readonly IConfiguration _configuration;
        private readonly DataBaseContext _dbContext;
        private readonly IJwtSettings _jwtSettings;
        public readonly UserManager<Users> _userManager;
        public LoginServicecs(ILogger<LoginServicecs> logger, IConfiguration configuration, DataBaseContext dataBaseContext, IJwtSettings jwtSettings, UserManager<Users> userManager)
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

                    Status = new ApiResponse
                    {
                       
                            FriendlyMessage = "Error occured!! Please try again later",
                            ErrorCode = errorCode,
                            IsSuccess =false,
                            TechnicalMessage = $"ErrorID : LoginAsync{errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}"
                        
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
                        Status = new ApiResponse
                        {
                            IsSuccess = false,
                            
                                FriendlyMessage = ex?.InnerException?.Message ?? ex?.Message ??"",
                            
                        }
                    };
                }

                return new AuthenticationResult
                {
                    Token = tokenHandler.WriteToken(token),
                    Status = new ApiResponse { IsSuccess = true }
                };
            }
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.LogError($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new AuthenticationResult
                {

                    Status = new ApiResponse
                    { 
                            IsSuccess = false,
                            FriendlyMessage = "Error occured!! Please try again later",
                            ErrorCode = errorCode,
                            TechnicalMessage = $"ErrorID :{errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}"
                        
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
