using TVShowApplication.Models;

namespace TVShowApplication.Services.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review?> GetReviewAsync(int id);
        Task<IEnumerable<Review>> GetReviewAsync();
        Task<int?> InsertReviewAsync(Review review);
        Task<bool> UpdateReviewAsync(int id, Review review);
        Task<bool> DeleteReviewAsync(int id);
    }
}
