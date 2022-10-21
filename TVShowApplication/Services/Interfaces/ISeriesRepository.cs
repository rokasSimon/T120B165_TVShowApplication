using TVShowApplication.Models;

namespace TVShowApplication.Services.Interfaces
{
    public interface ISeriesRepository
    {
        Task<Series?> GetSeriesAsync(int genreId, int seriesId);
        Task<IEnumerable<Series>> GetSeriesAsync(int genreId);
        Task<Series?> InsertSeriesAsync(int genreId, Series series);
        Task<bool> UpdateSeriesAsync(int genreId, int seriesId, Series series);
        Task<bool> DeleteSeriesAsync(int genreId, int seriesId);
    }
}
