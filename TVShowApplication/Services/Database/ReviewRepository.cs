using Microsoft.EntityFrameworkCore;
using TVShowApplication.Data;
using TVShowApplication.Models;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Services.Database
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly TVShowContext _context;

        public ReviewRepository(TVShowContext context)
        {
            _context = context;
        }

        public async Task<Review?> GetReviewAsync(int id)
        {
            return await _context.Reviews.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Review>> GetReviewAsync()
        {
            return await _context.Reviews.ToListAsync();
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var reviewToDelete = new Review { Id = id };

            _context.Reviews.Remove(reviewToDelete);

            return await SaveAsync();
        }

        public async Task<int?> InsertReviewAsync(Review review)
        {
            var createdReview = await _context.Reviews.AddAsync(review);
            var successfullyCreated = await SaveAsync();

            if (successfullyCreated)
            {
                return createdReview.Entity.Id;
            }

            return null;
        }

        public async Task<bool> UpdateReviewAsync(int id, Review review)
        {
            review.Id = id;

            _context.Reviews.Update(review);

            return await SaveAsync();
        }

        private async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() != 0;
        }
    }
}
