using static GOSBackend.Contracts.Admin_Objs.HelpObjs;
using static GOSBackend.Contracts.Common.AuxillaryObjs;

namespace GOSBackend.Repository.Interface
{
    public interface IAdminServices
    {
        Task<HelpAddResObj> AddUpdateHelpContent(HelpAddModel model);
        HelpAddResObjs GetAllContents();
        HelpAddResObj GetContent(int HelpId);
        Task<DeleteResObj> DeleteHelpContents(DeleteIds model);
    }
}
