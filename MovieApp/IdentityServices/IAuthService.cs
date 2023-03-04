using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Financial_Identity;
using static GOSBackend.Contracts.User_Identity_Obj.AdminIdentityObjs;

namespace GOSBackend.IdentityServices
{
    public interface IAuthService
    {
        Task<AuthenticationResult> RegisterAdmin(AdminRegisterObj model);
        Task<AuthResponse> VerifyAdminRegister(AdminVerifyObj request);
        Task<AuthResponse> LoginAdmin(LoginCommand request);
    }
}
