using Microsoft.EntityFrameworkCore;
using TVShowApplication.Data;
using TVShowApplication.Data.DTO.Genre;
using TVShowApplication.Models;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Services.Database
{
    public class GenreRepository : IGenreRepository
    {
        private readonly TVShowContext _context;

        public GenreRepository(TVShowContext context)
        {
            _context = context;
        }

        public async Task<Genre?> GetGenreAsync(int id)
        {
            return await _context.Genres.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Genre>> GetGenresAsync()
        {
            return await _context.Genres.ToListAsync();
        }

        public async Task<bool> DeleteGenreAsync(int id)
        {
            var genreToDelete = new Genre { Id = id };

            _context.Genres.Remove(genreToDelete);

            return await SaveAsync();
        }

        public async Task<Genre?> InsertGenreAsync(Genre genre)
        {
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
            var existingGenre = _context.Genres.SingleOrDefaultAsync(x => x.Id == id);
            if (existingGenre == null) return false;

            genre.Id = id;
            _context.Genres.Update(genre);

            return await SaveAsync();
        }

        private async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() != 0;
        }
    }
}
