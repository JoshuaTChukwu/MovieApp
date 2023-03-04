using Microsoft.AspNetCore.Identity;

namespace GOSBackend.SqlTables
{
    public class Users : IdentityUser
    {
        public int UserType { get; set; }
        public bool IsActivatad { get; set; }
        public bool AdminActivated { get; set; }
        public int Gender { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
