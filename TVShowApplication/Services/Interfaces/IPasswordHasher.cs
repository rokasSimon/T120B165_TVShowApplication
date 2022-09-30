namespace TVShowApplication.Services.Interfaces
{
    public interface IPasswordHasher
    {
        string CreateSalt();
        string HashPassword(string password, string salt);
        bool VerifyPassword(string hashedPassword, string password, string salt);
    }
}
