namespace TVShowApplication.Services.Interfaces
{
    public interface IJwtGenerator
    {
        string GenerateToken(IDictionary<string, string> claims);
    }
}
