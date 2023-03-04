using MovieApp.Data;
using MovieApp.Requests;
using MovieApp.SqlTables;
using Polly;
using static MovieApp.Contracts.Common.AuxillaryObjs;
using static MovieApp.Contracts.Common.OMDBAPIRequestObjs;
using static MovieApp.Contracts.User_Identity_Obj.UsersOperationalObjs;

namespace MovieApp.Repository.Implements
{
    public class UsersInterface
    {
        private readonly DataBaseContext _dBContext;
        private readonly IAPIHelper _api;
        private string _userId;
        public UsersInterface(DataBaseContext dBContext, IAPIHelper api, IHttpContextAccessor httpContextAccessor)
        {
            _dBContext = dBContext;
            _api = api;
            _userId = httpContextAccessor?.HttpContext?.User?.FindFirst("userId")?.Value ?? string.Empty;
        }

        public SearchResponseObj seachMovie(SearchParams search)
        {
            var response = new SearchResponseObj
            {
                Status = new ApiResponse
                {
                    IsSuccess = false
                }
            };
            var movies = _api.GetMovie(search).Result;
            if(movies.Response.ToLower() == "true")
            {
                response.Response = new SearcResponseModel
                {
                    Page = search.Page,
                    Search = movies.Search,
                };
                if(search.Page == 1)
                {
                    response.Response.HasPrev = false;
                }
                var newSearchEntry = new MovieSearch
                {
                    DateSearch = DateTime.Now,
                    MovieName = search.SearchValue,
                    UserId = _userId,
                };
                _dBContext.Add(newSearchEntry);
                if(movies.TotalResults > (search.Page * 10))
                {
                    response.Response.HasNext = true;
                }
                _dBContext.SaveChanges();
                response.Status.FriendlyMessage = "Search succesful";
                response.Status.IsSuccess = true;
                return response;
            }
            throw new Exception("Movie not found");
        }
        public IEnumerable<string> lastLatestSearch()
        {
            var response = (from a in _dBContext.MovieSearch
                            where a.UserId == _userId
                            orderby a.DateSearch descending
                            select a.MovieName).Take(5).ToList();
            return response;
        }
    }
}
