using static MovieApp.Contracts.User_Identity_Obj.IdentityObjs;

namespace MovieApp.IdentityServices
{
    public interface IProfileService
    {
        Task<UserProfileResObj> GetAdminProfile();
    }
}
