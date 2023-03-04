using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSBackend.Contracts.Common
{
    public static class APIRoutes
    {
        private const string Base = "api/v1";
        public static class Admin
        {
            public const string ADMIN_REGISTER = Base + "/admin/register"; 
            public const string ADMIN_LOGIN = Base + "/admin/login";
            public const string ADMIN_PROFILE_GET = Base + "/admin/profile/get";
            public const string ADMIN_VERIFY = Base + "/admin/verify";
            public const string ADD_HELP = Base + "/admin/helpcontents/add/update";
            public const string DELETE_HELP = Base + "/admin/helpcontents/delete";
            public const string GET_ALL_HELP = Base + "/admin/helpcontents/get/all";
            public const string GET_HELP = Base + "/admin/helpcontents/get/by/id";
        }
    }
}
