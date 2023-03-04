using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MovieApp.SqlTables
{
    public class RefreshToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Token { get; set; } = string.Empty;
        public string JwtId { get; set; } = String.Empty;
        public DateTime CreationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool Used { get; set; }
        public bool Invalidated { get; set; }
        public string UserId { get; set; }= string.Empty;
        [NotMapped]
        [ForeignKey(nameof(UserId))]
        public Users User { get; set; } = new Users();
    }
}
