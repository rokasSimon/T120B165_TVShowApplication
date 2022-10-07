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
            return await _context.Reviews
                .Include(r => r.Reviewer)
                .Include(r => r.ReviewedSeries)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Review>> GetReviewAsync()
        {
            return await _context.Reviews
                .Include(r => r.Reviewer)
                .Include(r => r.ReviewedSeries)
                .ToListAsync();
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var reviewToDelete = new Review { Id = id };

            _context.Reviews.Remove(reviewToDelete);

            return await SaveAsync();
        }

        public async Task<Review?> InsertReviewAsync(Review review)
        {
            if (review.Reviewer == null) return null;
            var reviewer = await _context.Users.SingleOrDefaultAsync(x => x.Id == review.Reviewer.Id);

            if (reviewer == null) return null;
            review.Reviewer = reviewer;

            var series = await _context.Series.SingleOrDefaultAsync(x => x.Id == review.ReviewedSeries.Id);
            if (series == null) return null;

            review.ReviewedSeries = series;

            var createdReview = await _context.Reviews.AddAsync(review);
            var successfullyCreated = await SaveAsync();

            if (successfullyCreated)
            {
                return createdReview.Entity;
            }

            return null;
        }

        public async Task<bool> UpdateReviewAsync(int id, Review review)
        {
            var reviewToUpdate = await _context.Reviews.SingleOrDefaultAsync(x => x.Id == id);
            if (reviewToUpdate == null) return false;

            reviewToUpdate.Text = review.Text;
            reviewToUpdate.Rating = review.Rating;

            _context.Reviews.Update(reviewToUpdate);

            return await SaveAsync();
        }

        private async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() != 0;
        }
    }
}
