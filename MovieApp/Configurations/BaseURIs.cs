namespace MovieApp.Configurations
{
    public class BaseURIs : IBaseURIs
    {
        public string LiveGateway { get; set; } = string.Empty;
        public string MainClient { get; set; } = string.Empty;
        public string OMDBAPI { get; set; } = string.Empty;
    }
    public interface IBaseURIs
    {
        string LiveGateway { get; set; }
        string MainClient { get; set; }
        string OMDBAPI { get; set; }
    }
}
