using static MovieApp.Contracts.Common.OMDBAPIRequestObjs;
using static MovieApp.Contracts.User_Identity_Obj.UsersOperationalObjs;

namespace MovieApp.Repository.Interface
{
    public interface IUsersInterface
    {
        SearchResponseObj seachMovie(SearchParams search);
        IEnumerable<string> lastLatestSearch();
    }
}
