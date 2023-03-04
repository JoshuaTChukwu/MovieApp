using static GOSBackend.Contracts.User_Identity_Obj.AdminIdentityObjs;

namespace GOSBackend.IdentityServices
{
    public interface IProfileService
    {
        Task<UserAdminProfileResObj> GetAdminProfile();
    }
}
