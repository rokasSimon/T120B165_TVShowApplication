using Microsoft.EntityFrameworkCore;
using TVShowApplication.Data;
using TVShowApplication.Models;
using TVShowApplication.Extensions;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Services.Database
{
    public class GenreRepository : IGenreRepository
    {
        private readonly IUserDataProvider _userDataProvider;
        private readonly TVShowContext _context;

        public GenreRepository(TVShowContext context, IUserDataProvider userDataProvider)
        {
            _context = context;
            _userDataProvider = userDataProvider;
        }

        public async Task<Genre?> GetGenreAsync(int id)
        {
            return await _context.Genres
                .Include(g => g.Videos)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Genre>> GetGenresAsync()
        {
            return await _context.Genres
                .Include(g => g.Videos)
                .ToListAsync();
        }

        public async Task<bool> DeleteGenreAsync(int id)
        {
            Fault.IfMissingRole(_userDataProvider.UserRole, Role.Admin);

            var genreToDelete = new Genre { Id = id };

            _context.Genres.Remove(genreToDelete);

            return await SaveAsync();
        }

        public async Task<Genre?> InsertGenreAsync(Genre genre)
        {
            Fault.IfMissingRole(_userDataProvider.UserRole, Role.Admin);

            var createdGenre = await _context.Genres.AddAsync(genre);
            var successfullyCreated = await SaveAsync();

            if (successfullyCreated)
            {
                return createdGenre.Entity;
            }

            return null;
        }

        public async Task<bool> UpdateGenreAsync(int id, Genre genre)
        {
            Fault.IfMissingRole(_userDataProvider.UserRole, Role.Admin);

            var existingGenre = await _context.Genres.SingleOrDefaultAsync(x => x.Id == id);
            if (existingGenre == null) return false;

            existingGenre.Description = genre.Description;

            _context.Genres.Update(existingGenre);

            return await SaveAsync();
        }

        private async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() != 0;
        }
    }
}
