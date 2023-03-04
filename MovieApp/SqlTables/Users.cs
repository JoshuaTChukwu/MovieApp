using Microsoft.AspNetCore.Identity;

namespace MovieApp.SqlTables
{
    public class Users : IdentityUser
    {
       
        public string FullName { get; set; } = string.Empty;
       
    }
}
