using Microsoft.EntityFrameworkCore;
using TVShowApplication.Data;
using TVShowApplication.Models;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Services.Database
{
    public class SeriesRepository : ISeriesRepository
    {
        private readonly TVShowContext _context;

        public SeriesRepository(TVShowContext context)
        {
            _context = context;
        }

        public async Task<Series?> GetSeriesAsync(int id)
        {
            return await _context.Series
                .Include(s => s.Reviews)
                .Include(s => s.Genres)
                .Include(s => s.Poster)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Series>> GetSeriesAsync()
        {
            return await _context.Series
                .Include(s => s.Genres)
                .Include(s => s.Poster)
                .ToListAsync();
        }

        public async Task<bool> DeleteSeriesAsync(int id)
        {
            var seriesToDelete = new Series { Id = id };

            _context.Series.Remove(seriesToDelete);

            return await SaveAsync();
        }

        public async Task<Series?> InsertSeriesAsync(Series series)
        {
            var genreIds = series.Genres.Select(series => series.Id);
            var genresFromDb = await _context.Genres
                .Where(g => genreIds.Contains(g.Id))
                .ToListAsync();

            if (series.Genres.Count != genresFromDb.Count) return null;
            series.Genres = genresFromDb;

            var posterFromDb = await _context.Posters.SingleOrDefaultAsync(p => p.Id == series.Poster.Id);
            if (posterFromDb == null) return null;
            series.Poster = posterFromDb;

            var createdSeries = await _context.Series.AddAsync(series);
            var successfullyCreated = await SaveAsync();

            if (successfullyCreated)
            {
                return createdSeries.Entity;
            }

            return null;
        }

        public async Task<bool> UpdateSeriesAsync(int id, Series series)
        {
            var seriesToUpdate = await _context.Series.SingleOrDefaultAsync(x => x.Id == id);
            if (seriesToUpdate == null) return false;

            seriesToUpdate.Description = series.Description;
            seriesToUpdate.Directors = series.Directors;
            seriesToUpdate.StarringCast = series.StarringCast;

            _context.Series.Update(seriesToUpdate);

            return await SaveAsync();
        }

        private async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() != 0;
        }
    }
}
