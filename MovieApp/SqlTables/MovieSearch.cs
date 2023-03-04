using System.ComponentModel.DataAnnotations;

namespace MovieApp.SqlTables
{
    public class MovieSearch
    {
        [Key]
        public int MovieSearchId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string MovieName { get; set; } = string.Empty;
        public DateTime DateSearch { get; set; } = DateTime.Now;
        public Users User { get; set; } = new Users();
    }
}
