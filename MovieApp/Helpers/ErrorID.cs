namespace MovieApp.Helpers
{
    public static class ErrorID
    {
        public static string Generate(int digit)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, digit)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
