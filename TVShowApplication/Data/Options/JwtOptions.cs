namespace TVShowApplication.Data.Options
{
    public class JwtOptions
    {
        public const string Options = nameof(Options);

        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? Secret { get; set; }
        public int? ExpirationSeconds { get; set; }
    }
}
