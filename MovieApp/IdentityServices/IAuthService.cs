using static MovieApp.Contracts.Common.AuxillaryObjs;
using static MovieApp.Contracts.User_Identity_Obj.IdentityObjs;

namespace MovieApp.IdentityServices
{
    public interface IAuthService
    {
        Task<AuthenticationResult> RegisterAdmin(RegisterObj model);
        Task<AuthResponse> VerifyAdminRegister(VerifyObj request);
        Task<AuthResponse> LoginAdmin(LoginCommand request);
    }
}
