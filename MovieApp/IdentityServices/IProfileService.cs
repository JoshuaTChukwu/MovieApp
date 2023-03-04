using static MovieApp.Contracts.User_Identity_Obj.IdentityObjs;

namespace GOSBackend.IdentityServices
{
    public interface IProfileService
    {
        Task<UserProfileResObj> GetAdminProfile();
    }
}
