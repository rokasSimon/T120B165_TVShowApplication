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
            return await _context.Series.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Series>> GetSeriesAsync()
        {
            return await _context.Series.ToListAsync();
        }

        public async Task<bool> DeleteSeriesAsync(int id)
        {
            var seriesToDelete = new Series { Id = id };

            _context.Series.Remove(seriesToDelete);

            return await SaveAsync();
        }

        public async Task<int?> InsertSeriesAsync(Series series)
        {
            var createdSeries = await _context.Series.AddAsync(series);
            var successfullyCreated = await SaveAsync();

            if (successfullyCreated)
            {
                return createdSeries.Entity.Id;
            }

            return null;
        }

        public async Task<bool> UpdateSeriesAsync(int id, Series series)
        {
            series.Id = id;

            _context.Series.Update(series);

            return await SaveAsync();
        }

        private async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() != 0;
        }
    }
}
