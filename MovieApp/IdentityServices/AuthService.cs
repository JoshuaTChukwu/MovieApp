using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MovieApp.Configurations;
using MovieApp.Data;
using MovieApp.Handlers;
using MovieApp.Helpers;
using MovieApp.Requests;
using MovieApp.SqlTables;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static MovieApp.Contracts.Common.AuxillaryObjs;
using static MovieApp.Contracts.User_Identity_Obj.IdentityObjs;

namespace MovieApp.IdentityServices
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Users> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<IAuthService> _logger;
        private readonly IEmailHelper _email;
        private readonly IJwtSettings _jwtSettings;
        private readonly DataBaseContext _dbContext;
        private readonly ILoginServices _login;
        private readonly IBaseURIs _uri;
        public AuthService(UserManager<Users> userManager, IWebHostEnvironment environment, ILogger<IAuthService> logger, IEmailHelper emailHelper
, IJwtSettings jwtSettings, DataBaseContext dataBaseContext,  ILoginServices services, IBaseURIs baseURIs          )
        {
            _userManager = userManager;
            _environment = environment;
            _logger = logger;
            _uri = baseURIs;
            _email = emailHelper;
            _jwtSettings = jwtSettings;
            _dbContext = dataBaseContext;
            _login = services;
        }
        public async Task<AuthenticationResult> RegisterAdmin(RegisterObj model)
        {
            var response = new AuthenticationResult() {Status =new ApiResponse { IsSuccess = false,FriendlyMessage =""  } };
            try
            {
               
                 var checkEmail = await _userManager.FindByEmailAsync(model.Email);
                 if(checkEmail != null)
                {
                    response.Status.FriendlyMessage = "Email alrady exists";
                    return response;
                }
                var checkUsername = await _userManager.FindByNameAsync(model.UserName);
                if(checkUsername != null)
                {
                    response.Status.FriendlyMessage = "Username already exists";
                    return response;
                }
                var newUser = new Users
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    FullName = model.FullName,
                    EmailConfirmed = false,
                   
                   
                };
                var register = await _userManager.CreateAsync(newUser,model.Password);
                if(register.Succeeded == false)
                {
                    response.Status.FriendlyMessage = register.Errors.FirstOrDefault()?.Description ??"error occured";
                    return response;
                }
                response.Status.IsSuccess = true;
                response.Status.FriendlyMessage = "Users registration is sucessful, kindly check ur email for comfirmation";
                var filePath = String.Concat(_environment.WebRootPath, "/mailTemplateReg.html");
                var claims = new List<Claim>
                            {

                                new Claim("email",model.Email),
                                new Claim("securedpin","SecurerPassage@#123")
                            };
                var token = CreateToken(claims);
                var link = $"{_uri.MainClient}/verify?g={token}";
                var msg = File.ReadAllText(filePath);
                msg = msg.Replace("{Name}", model.FullName).Replace("{Link}", link);
                var sendMail = _email.SendMail(model.Email, model.FullName, "Verification Email",msg);
                return response;
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.LogError($"ErrorID : {errorCode} Ex : {ex?.InnerException?.Message ?? ex?.Message} ErrorStack : {ex?.StackTrace}");
                response.Status.IsSuccess = false;
                response.Status.FriendlyMessage = $"An error occured, Kindly submit this issue with id {errorCode} to the Admin";
                response.Status.TechnicalMessage = ex?.InnerException?.Message ?? ex?.Message ?? "";
                response.Status.ErrorCode = errorCode;
                return response;
            }
        }

        public async Task<AuthResponse> VerifyAdminRegister(VerifyObj request)
        {
            var response = new AuthResponse { Status = new ApiResponse { IsSuccess = false, } };
            try
            {
                string token = request.Token.Replace("Bearer ", "").Trim();
                var handler = new JwtSecurityTokenHandler();
                var tokena = handler.ReadJwtToken(token);
                var securedPin = tokena.Claims.First(x => x.Type == "securedpin");
                var email = tokena.Claims.First(x => x.Type == "email");
                if (securedPin.Value == null || securedPin.Value != "SecurerPassage@#123" || email.Value == null)
                {
                    response.Status.FriendlyMessage = "verification failed, please try again";
                    return response;
                }
                if (tokena.ValidTo < DateTime.Now)
                {
                    response.Status.FriendlyMessage = "Verification link expired, please try again";
                    return response;
                }
                var user_account = await _userManager.FindByEmailAsync(email.Value);
                if (user_account == null)
                {
                    response.Status.FriendlyMessage = "User Account does not exist";
                    return response;
                }

               
                user_account.EmailConfirmed = true;
              
                await _userManager.UpdateAsync(user_account);
                var content3 = new StringBuilder();
                content3.Append("<p> Your account has been validated</p>");
                content3.Append("<p>You can now login</p>");
                
                var body2 =File.ReadAllText(String.Concat(_environment.WebRootPath, "/mailTemplateWithButton.html"));
                var link = $"{_uri.MainClient}/login";
                body2 = body2.Replace("{Name}", user_account.FullName).Replace("{Link}", link).Replace("{Body}", content3.ToString()).Replace("{Button}", "Login");
                var mail = _email.SendMail(user_account.Email, user_account.FullName, "Account Activation", body2);
                response.Status.IsSuccess = true;
                response.Status.FriendlyMessage = "Account activated";
                return response;
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.LogError($"ErrorID : {errorCode} Ex : {ex?.InnerException?.Message ?? ex?.Message} ErrorStack : {ex?.StackTrace}");
                response.Status.FriendlyMessage = $"An error occured, Kindly submit this issue with id {errorCode} to the Admin" ;
                response.Status.TechnicalMessage = ex?.InnerException?.Message ?? ex?.Message ?? "";
                response.Status.ErrorCode = errorCode;
                return response;
            }
        }
        private string CreateToken(List<Claim> claims)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

                var tokenDecriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifeSpan),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };


                var token = tokenHandler.CreateToken(tokenDecriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<AuthResponse> LoginAdmin(LoginCommand request)
        {
            var response = new AuthResponse { Status = new ApiResponse { IsSuccess = false,  } };
            try
            {
                Users? user_acccount = new Users();
                if (IsValidEmail(request.UserName))
                {
                    user_acccount = await _userManager.FindByEmailAsync(request.UserName) ?? null;
                }
                else
                {
                    user_acccount = await _userManager.FindByNameAsync(request.UserName) ?? null;
                }
                
               
              




                if (!await UserExist(request, user_acccount))
                {
                    response.Status.FriendlyMessage = "User does not exist";
                    return response;
                }
                Users users = user_acccount ?? new Users();
                if (!await IsValidPassword(request, users))
                {
                    response.Status.FriendlyMessage = "User/Password Combination is wrong";
                    return response;
                }

               
                if(users.EmailConfirmed == false)
                {
                    response.Status.FriendlyMessage = "Kindly Comfirm your Email";
                    return response;
                }
                



                var result = await _login.ClientLoginAsync(users);
                if (result.Status.IsSuccess == false)
                {
                    response.Status.IsSuccess = false;
                    response.Status.FriendlyMessage = result.Status.FriendlyMessage;
                    response.Status.TechnicalMessage = result.Status.TechnicalMessage;
                    return response;
                }

                response.Status.IsSuccess = true;
                response.Token = result.Token;
                return response;
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.LogError($"ErrorID : {errorCode} Ex : {ex?.InnerException?.Message ?? ex?.Message} ErrorStack : {ex?.StackTrace}");
                response.Status.FriendlyMessage = $"An error occured, Kindly submit this issue with id {errorCode} to the Admin";
                response.Status.TechnicalMessage = ex?.InnerException?.Message ?? ex?.Message ?? "";
                response.Status.ErrorCode = errorCode;
                return response;
            }
        }
        private async Task<bool> UserExist(LoginCommand request, Users? user)
        {
            if (user == null)
            {
                return await Task.Run(() => false);
            }
            return await Task.Run(() => true);
        }
        private async Task<bool> IsValidPassword(LoginCommand request, Users user)
        {
            var isValidPass = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isValidPass)
            {
                return await Task.Run(() => false);
            }
            return await Task.Run(() => true);
        }
        private bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

    }
}
