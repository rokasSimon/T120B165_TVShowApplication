using Microsoft.EntityFrameworkCore;
using TVShowApplication.Data;
using TVShowApplication.Exceptions;
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

            var existingUser = await set.SingleOrDefaultAsync(x => x.Id == user.Id);
            if (existingUser != null) return null;

            var createdUser = await set.AddAsync(user);
            await SaveAsync();

            return createdUser.Entity;
        }

        public async Task<bool> UpdateUserAsync<T>(int id, T user) where T : User
        {
            var set = _context.Set<T>();

            var userFromDb = await set.SingleOrDefaultAsync(x => x.Id == id);
            if (userFromDb == null) throw new ResourceNotFoundException("No such user exists.");

            userFromDb.Email = user.Email;

            set.Update(userFromDb);

            return await SaveAsync();
        }

        public async Task<bool> DeleteUserAsync<T>(int id) where T : User
        {
            var set = _context.Set<T>();

            var userToBeDeleted = await set.SingleOrDefaultAsync(x => x.Id == id);
            if (userToBeDeleted == null) throw new ResourceNotFoundException("No such user exists.");

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
