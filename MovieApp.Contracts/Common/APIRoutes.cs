using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.Contracts.Common
{
    public static class APIRoutes
    {
        private const string Base = "api/v1";
        public static class Users
        {
            public const string USER_REGISTER = Base + "/users/register"; 
            public const string USER_LOGIN = Base + "/users/login";
            public const string USER_PROFILE_GET = Base + "/users/profile/get";
            public const string USER_VERIFY = Base + "/users/verify";
            public const string SEARCH_MOVIE = Base + "/users/search/movie/get";
            public const string SINGLE_MOVIE = Base + "/users/single/movie/get";
            public const string LATEST_REQUEST = Base + "/users/latest/searched/get";

        }
    }
}
