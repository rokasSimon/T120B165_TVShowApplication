using TVShowApplication.Models;

namespace TVShowApplication.Services.Interfaces
{
    public interface IUserDataProvider
    {
        public Role UserRole { get; set; }
        public int UserId { get; set; }
    }
}
