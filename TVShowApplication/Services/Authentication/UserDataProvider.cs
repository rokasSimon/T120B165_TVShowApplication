using TVShowApplication.Models;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Services.Authentication
{
    public class UserDataProvider : IUserDataProvider
    {
        public Role UserRole { get; set; }
        public int UserId { get; set; }
    }
}
