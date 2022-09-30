using System.Text;
using System.Security.Cryptography;
using Konscious.Security.Cryptography;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Services.Authentication
{
    public class Argon2idPasswordHasher : IPasswordHasher
    {
        public string CreateSalt()
        {
            var salt = RandomNumberGenerator.GetBytes(16);

            return Encoding.UTF8.GetString(salt);
        }

        public string HashPassword(string password, string salt)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltBytes = Encoding.UTF8.GetBytes(salt);

            return Encoding.UTF8.GetString(HashPassword(passwordBytes, saltBytes));
        }

        public bool VerifyPassword(string hashedPassword, string password, string salt)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltBytes = Encoding.UTF8.GetBytes(salt);

            var passwordHash = HashPassword(passwordBytes, saltBytes);
            var hashedPasswordBytes = Encoding.UTF8.GetBytes(hashedPassword);

            return passwordHash.SequenceEqual(hashedPasswordBytes);
        }

        private byte[] HashPassword(byte[] password, byte[] salt)
        {
            var argon = new Argon2id(password)
            {
                MemorySize = 30000,
                Iterations = 20,
                DegreeOfParallelism = 2,
                Salt = salt
            };

            var hash = argon.GetBytes(128);

            return hash;
        }
    }
}
