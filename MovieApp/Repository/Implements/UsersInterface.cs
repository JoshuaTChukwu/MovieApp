using Microsoft.AspNetCore.Identity;
using MovieApp.Data;
using MovieApp.Repository.Interface;
using MovieApp.Requests;
using MovieApp.SqlTables;
using Polly;
using static MovieApp.Contracts.Common.AuxillaryObjs;
using static MovieApp.Contracts.Common.OMDBAPIRequestObjs;
using static MovieApp.Contracts.User_Identity_Obj.UsersOperationalObjs;

namespace MovieApp.Repository.Implements
{
    public class UsersInterface : IUsersInterface
    {
        private readonly DataBaseContext _dBContext;
        private readonly IAPIHelper _api;
        private string _userId;
        private readonly UserManager<Users> _user;
        public UsersInterface(DataBaseContext dBContext, IAPIHelper api, IHttpContextAccessor httpContextAccessor, UserManager<Users> userManager)
        {
            _dBContext = dBContext;
            _api = api;
            _userId = httpContextAccessor?.HttpContext?.User?.FindFirst("userId")?.Value ?? string.Empty;
            _user = userManager;
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
                var user =  _user.FindByIdAsync(_userId).Result;
                var newSearchEntry = new MovieSearch
                {
                    DateSearch = DateTime.Now,
                    MovieName = search.SearchValue,
                    User = user,
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

        public SingleSearchResponseObj GetMovie(string searchId)
        {
            var response = new SingleSearchResponseObj
            {
                Status = new ApiResponse
                {
                    IsSuccess = false
                }
            };
            var movies = _api.GetSingleMovie(searchId).Result;
            if (movies.Response.ToLower() == "true")
            {
                response.Response = movies;
               
                response.Status.FriendlyMessage = "Search succesful";
                response.Status.IsSuccess = true;
                return response;
            }
            throw new Exception("Movie not found");
        }
        public QueriesSearched lastLatestSearch()
        {
            var result = (from a in _dBContext.MovieSearch
                            where a.UserId == _userId
                            orderby a.DateSearch descending
                            select a.MovieName).Distinct().Take(5).ToList();
            var response = new QueriesSearched
            {
                Queries = result,
                Status = new ApiResponse { IsSuccess = true }
            };
            return response;
        }
    }
}
