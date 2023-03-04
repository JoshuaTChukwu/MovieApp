using static MovieApp.Contracts.Common.AuxillaryObjs;
using static MovieApp.Contracts.User_Identity_Obj.IdentityObjs;

namespace GOSBackend.IdentityServices
{
    public interface IAuthService
    {
        Task<AuthenticationResult> RegisterAdmin(RegisterObj model);
        Task<AuthResponse> VerifyAdminRegister(AdminVerifyObj request);
        Task<AuthResponse> LoginAdmin(LoginCommand request);
    }
}
