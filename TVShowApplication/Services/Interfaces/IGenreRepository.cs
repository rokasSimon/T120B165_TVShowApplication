using TVShowApplication.Models;

namespace TVShowApplication.Services.Interfaces
{
    public interface IGenreRepository
    {
        Task<Genre?> GetGenreAsync(int id);
        Task<IEnumerable<Genre>> GetGenresAsync();
        Task<int?> InsertGenreAsync(Genre genre);
        Task<bool> UpdateGenreAsync(int id, Genre genre);
        Task<bool> DeleteGenreAsync(int id);
    }
}
