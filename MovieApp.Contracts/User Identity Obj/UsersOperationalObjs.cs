using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MovieApp.Contracts.Common.AuxillaryObjs;
using static MovieApp.Contracts.Common.OMDBAPIRequestObjs;

namespace MovieApp.Contracts.User_Identity_Obj
{
    public class UsersOperationalObjs
    {
       public class SearcResponseModel
        {

            public bool HasNext { get; set; } = false;
            public bool HasPrev { get; set; } = true;
            public int Page { get; set; } = 1;
            public IEnumerable<SearchItem> Search { get; set; } = Enumerable.Empty<SearchItem>();
        }
        public class SearchResponseObj
        {
           public SearcResponseModel Response { get; set; } = new SearcResponseModel();

           public ApiResponse Status { get; set; } = new ApiResponse();
        }
    }
}
