namespace MovieApp.Configurations
{
    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public TimeOnly TokenLifeSpan { get; set; } = new TimeOnly();
    }
    public interface IJwtSettings
    {
        string Secret { get; set; }
        TimeOnly TokenLifeSpan { get; set; }
    }
}
