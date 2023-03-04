namespace MovieApp.SqlTables
{
    public class MovieSearch
    {
        public string UserId { get; set; } = string.Empty;
        public string MovieName { get; set; } = string.Empty;
        public DateTime DateSearch { get; set; } = DateTime.Now;
    }
}
