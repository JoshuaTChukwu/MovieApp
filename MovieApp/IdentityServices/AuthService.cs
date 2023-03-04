using GOSBackend.Handlers;
using GOSBackend.Requests;
using GOSBackend.SqlTables;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MovieApp.Configurations;
using MovieApp.Data;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static MovieApp.Contracts.Common.AuxillaryObjs;
using static MovieApp.Contracts.User_Identity_Obj.IdentityObjs;

namespace GOSBackend.IdentityServices
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
        public AuthService(UserManager<Users> userManager, IWebHostEnvironment environment, ILogger<IAuthService> logger, IEmailHelper emailHelper
, IJwtSettings jwtSettings, DataBaseContext dataBaseContext,  ILoginServices services          )
        {
            _userManager = userManager;
            _environment = environment;
            _logger = logger;
         
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
                    IsActivatad = false,
                   
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
                                new Claim("securedpin","GOSHardcoreSecurer@#123")
                            };
                var token = CreateToken(claims);
                var link = $"{_uri.SelfClient}/verify?g={token}";
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
                response.Status.FriendlyMessage = "An Unexpected error occured";
                response.Status.TechnicalMessage = ex?.Message;
                return response;
            }
        }

        public async Task<AuthResponse> VerifyAdminRegister(AdminVerifyObj request)
        {
            var response = new AuthResponse { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
            try
            {
                string token = request.Token.Replace("Bearer ", "").Trim();
                var handler = new JwtSecurityTokenHandler();
                var tokena = handler.ReadJwtToken(token);
                var securedPin = tokena.Claims.First(x => x.Type == "securedpin");
                var email = tokena.Claims.First(x => x.Type == "email");
                if (securedPin.Value == null || securedPin.Value != "GOSHardcoreSecurer@#123" || email.Value == null)
                {
                    response.Status.Message.FriendlyMessage = "verification failed, please try again";
                    return response;
                }
                if (tokena.ValidTo < DateTime.Now)
                {
                    response.Status.Message.FriendlyMessage = "Verification link expired, please try again";
                    return response;
                }
                var user_account = await _userManager.FindByEmailAsync(email.Value);
                if (user_account == null)
                {
                    response.Status.Message.FriendlyMessage = "User Account does not exist";
                    return response;
                }

                if (request.TokenType == TokenType.AdminUser)
                {
                    user_account.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user_account);
                    var claims = new List<Claim>
                            {

                                new Claim("email",email.Value),
                                new Claim("securedpin","GOSHardcoreSecurer@#123"),
                                new Claim("IsAdmin","true")
                            };


                    var newToken = CreateToken(claims);
                    var name = "Admin";
                    var baseUrl = _uri.SelfClient;

                    var url = baseUrl + "/verify?g=" + newToken + "&type=2";
                    var content = new StringBuilder();
                    content.Append($"<p>Kindly comfirm that {user_account.FullName} with email: {user_account.Email} is a licensed user. Click the button below </p>");
                    content.Append("<p>Once verified. Users will become admin </p>");
                    var filePath = String.Concat(_environment.WebRootPath, "/mailTemplateWithButton.html");
                    var body =  File.ReadAllText(filePath);
                    body = body.Replace("{Name}", name).Replace("{Link}", url).Replace("{Body}", content.ToString()).Replace("{Button}", "Verify");

                    var list = "abimbola.okekumata@godp.co.uk;samuel.yekini@godp.co.uk;info@godp.com.ng";
                    var addressList = list.Split(";").ToList();
                    foreach(var address in addressList)
                    {
                        var res = _email.SendMail(address, name, "Admin Verification", body);
                    }
                   
                    response.Status.Message.FriendlyMessage = "Account verified, Request sent to admin to activate";
                    response.Status.IsSuccessful = true;
                    return response;
                }
                var isAdmin = tokena.Claims.First(x => x.Type == "IsAdmin");
                if (isAdmin == null || isAdmin.Value != "true")
                {
                    response.Status.Message.FriendlyMessage = "This request is not from an admin";
                }
                user_account.AdminActivated = true;
                user_account.IsActivatad = true;
                await _userManager.UpdateAsync(user_account);
                var content3 = new StringBuilder();
                content3.Append("<p> Your account has been validated</p>");
                content3.Append("<p>You can now login to the admin page</p>");
                
                var body2 =File.ReadAllText(String.Concat(_environment.WebRootPath, "/mailTemplateWithButton.html"));
                var link = $"{_uri.SelfClient}/login";
                body2 = body2.Replace("{Name}", user_account.FullName).Replace("{Link}", link).Replace("{Body}", content3.ToString()).Replace("{Button}", "Login");
                var mail = _email.SendMail(user_account.Email, user_account.FullName, "Account Activation", body2);
                response.Status.IsSuccessful = true;
                response.Status.Message.FriendlyMessage = "Account activated";
                return response;
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.LogError($"ErrorID : {errorCode} Ex : {ex?.InnerException?.Message ?? ex?.Message} ErrorStack : {ex?.StackTrace}");
                response.Status.Message.FriendlyMessage = ex?.Message ?? ex?.InnerException?.Message;
                response.Status.Message.TechnicalMessage = ex?.ToString();
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
            var response = new AuthResponse { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
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
                    response.Status.Message.FriendlyMessage = "User does not exist";
                    return response;
                }
                Users users = user_acccount ?? new Users();
                if (!await IsValidPassword(request, users))
                {
                    response.Status.Message.FriendlyMessage = "User/Password Combination is wrong";
                    return response;
                }

                if ((UserType)users.UserType != UserType.Admin)
                {
                    response.Status.Message.FriendlyMessage = "User  not lincesed to login here";
                    return response;
                }
                if(users.EmailConfirmed == false)
                {
                    response.Status.Message.FriendlyMessage = "Kindly Comfirm your Email";
                    return response;
                }
                if (users.IsActivatad == false)
                {
                    response.Status.Message.FriendlyMessage = "Account not yet activated, see admin for details";
                    return response;
                }



                var result = await _login.ClientLoginAsync(users);
                if (result.Status.IsSuccessful == false)
                {
                    response.Status.IsSuccessful = false;
                    response.Status.Message = result.Status.Message;
                    return response;
                }

                response.Status.IsSuccessful = true;
                response.Token = result.Token;
                response.RefreshToken = result.RefreshToken;
                return response;
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.LogError($"ErrorID : {errorCode} Ex : {ex?.InnerException?.Message ?? ex?.Message} ErrorStack : {ex?.StackTrace}");
                response.Status.Message.FriendlyMessage = ex?.Message ?? ex?.InnerException?.Message;
                response.Status.Message.TechnicalMessage = ex?.ToString();
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
