namespace MovieApp.Configurations
{
    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public TimeSpan TokenLifeSpan { get; set; } = new TimeSpan();
    }
    public interface IJwtSettings
    {
        string Secret { get; set; }
        TimeSpan TokenLifeSpan { get; set; }
    }
}
