using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.Contracts.Common
{
    public class AuxillaryObjs
    {

        public class ApiResponse
        {
            public bool IsSuccess { get; set; }
            public string FriendlyMessage { get; set; } = string.Empty;
            public string TechnicalMessage { get; set; } = string.Empty;
        }
        public class AuthenticationResult
        {
            public ApiResponse Status { get; set; } = new ApiResponse();
        }
        public enum Gender
        {
            Male =1,
            Female,
            NonGender
        }
        public enum UserType
        {
            Admin =1,
            Partner,
            SoftwareClients
        }
        public enum FileType
        {
            None,
            PDF ,
            Word,
            JPEG,
            PNG,
            Excel
        }
        public enum Application
        {
            GOSERP = 1,
            GOSHRM,
            GOSFR,
            GOSRMS
        }
        public class CloudinarySettings
        {
            public string CloudName { get; set; } = string.Empty;
            public string ApiKey { get; set; } = string.Empty;
            public string ApiSecret { get; set; } = string.Empty;
        }
        public class DeleteIds
        {
            public IEnumerable<int> Ids { get; set; } = Enumerable.Empty<int>();
        }
        public class DeleteResObj
        {
            public bool IsDeleted { get; set; }
            public ApiResponse Status { get; set; } = new ApiResponse();
        }
    }
}
