using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.Contracts.Common
{
    public class OMDBAPIRequestObjs
    {
        public class SearchItem
        {
            public string Title { get; set; } = string.Empty;
            public string Year { get; set; } = string.Empty ;
            public string imdbID { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string Poster { get; set; } = string.Empty;
        }

        public class OmbdResult
        {
            public IEnumerable<SearchItem> Search { get; set; } = Enumerable.Empty<SearchItem>();
            public string Response { get; set; } = "False";
            public int TotalResults { get; set; } = 0;
        }
        public class SearchParams
        {
            public string SearchValue { get; set; } = string.Empty;
            public int Page { get; set; } = 1;
        }

    }
}
