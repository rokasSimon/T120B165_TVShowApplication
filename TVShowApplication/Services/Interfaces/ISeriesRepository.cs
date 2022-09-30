using TVShowApplication.Models;

namespace TVShowApplication.Services.Interfaces
{
    public interface ISeriesRepository
    {
        Task<Series?> GetSeriesAsync(int id);
        Task<IEnumerable<Series>> GetSeriesAsync();
        Task<Series?> InsertSeriesAsync(Series series);
        Task<bool> UpdateSeriesAsync(int id, Series series);
        Task<bool> DeleteSeriesAsync(int id);
    }
}
