using GOSLibraries.GOS_API_Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GOSBackend.Contracts.Common.AuxillaryObjs;

namespace GOSBackend.Contracts.Admin_Objs
{
    public class HelpObjs
    {
        public enum HelpOperationType
        {
            SetUp =1,
            Operations
        }
        public class HelpAddResObj
        {
            public HelpAddModel Help { get; set; } = new HelpAddModel();
            public APIResponseStatus Status { get; set; } = new APIResponseStatus();
        }

        public class HelpAddResObjs
        {
            public IEnumerable<HelpAddModel> Helps { get; set; } = Enumerable.Empty<HelpAddModel>();
            public APIResponseStatus Status { get; set; } = new APIResponseStatus();
        }
        public class HelpAddModel
        {
            public int HelpId { get; set; }
            public Application  Application { get; set; }
            public string Module { get; set; } = string.Empty;
            public HelpOperationType OperationType { get; set; }
            public string ProcessTitle { get; set; } = string.Empty;
            public string ProcessDescription { get; set; } = string.Empty;
            public IEnumerable<HelpTaskObj> Tasks { get; set; } = Enumerable.Empty<HelpTaskObj>();
        }
        public class HelpTaskObj
        {
            public string Instructition { get; set; } = string.Empty ;
            public bool HasImage { get; set; }
            public string ImageBase64 { get; set; } = string.Empty;
            public string ImageUrl { get; set; } = string.Empty;
            public string PublicId { get; set; } = string.Empty;
            public string ImageExtension { get; set; } = string.Empty;
        }
    }
}
