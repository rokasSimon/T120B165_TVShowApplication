using Microsoft.EntityFrameworkCore;
using TVShowApplication.Data;
using TVShowApplication.Models;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Services.Database
{
    public class UserRepository : IUserRepository
    {
        private readonly TVShowContext _context;

        public UserRepository(TVShowContext context)
        {
            _context = context;
        }

        public async Task<T?> GetUserAsync<T>(int id) where T : User
        {
            var set = _context.Set<T>();

            var user = await set.SingleOrDefaultAsync(x => x.Id == id);

            return user;
        }

        public async Task<IEnumerable<T>> GetUsersAsync<T>() where T : User
        {
            var set = _context.Set<T>();

            var users = await set.ToListAsync();

            return users;
        }

        public async Task<T?> InsertUserAsync<T>(T user) where T : User
        {
            var set = _context.Set<T>();

            var createdUser = await set.AddAsync(user);
            var successfullyCreated = await SaveAsync();

            if (successfullyCreated)
            {
                return createdUser.Entity;
            }

            return null;
        }

        public async Task<bool> UpdateUserAsync<T>(int id, T user) where T : User
        {
            var set = _context.Set<T>();

            user.Id = id;
            set.Update(user);

            return await SaveAsync();
        }

        public async Task<bool> DeleteUserAsync<T>(int id) where T : User
        {
            var userToBeDeleted = new User { Id = id };

            _context.Users.Remove(userToBeDeleted);

            return await SaveAsync();
        }

        private async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() != 0;
        }

        public async Task<User?> FindUserAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.Email == email);
        }
    }
}
