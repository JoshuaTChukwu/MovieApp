using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MovieApp.Contracts.Common.AuxillaryObjs;

namespace MovieApp.Contracts.User_Identity_Obj
{
    public class IdentityObjs
    {
        public class RegisterObj
        {
            public string UserName { get; set; }= string.Empty;
            public string Password { get; set; }= string.Empty;
            public string Email { get; set; }= string.Empty;
            public string FullName { get; set; }= string.Empty;
        }
        public class VerifyObj
        {
            public string Token { get; set; }= string.Empty;
          
        }
        public class LoginCommand
        {
            public string UserName { get; set; }= string.Empty;
            public string Password { get; set; }= string.Empty;
        }


        public class UserProfileObj
        {
            public string FullName { get; set; }= string.Empty;
            public string Gender { get; set; }= string.Empty;
            public string Email { get; set; }= string.Empty;
        }
        public class UserProfileResObj
        {
            public UserProfileObj Profile {get; set; } = new UserProfileObj();
            public ApiResponse Status { get; set; } = new ApiResponse();
        }
    }
}
