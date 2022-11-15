using Microsoft.EntityFrameworkCore;
using TVShowApplication.Data;
using TVShowApplication.Exceptions;
using TVShowApplication.Models;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Services.Database
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly IUserDataProvider _userDataProvider;
        private readonly TVShowContext _context;

        public ReviewRepository(TVShowContext context, IUserDataProvider userDataProvider)
        {
            _context = context;
            _userDataProvider = userDataProvider;
        }

        public async Task<Review> GetReviewAsync(int genreId, int seriesId, int reviewId)
        {
            var review = await _context.Reviews
                .Include(r => r.ReviewedSeries)
                    .ThenInclude(s => s.Genres)
                .Include(r => r.Reviewer)
                .SingleOrDefaultAsync(x => x.Id == reviewId);
            
            if (review == null) throw new ResourceNotFoundException($"There is no such review: '{reviewId}'.");
            if (review.ReviewedSeries.Id != seriesId) throw new ResourceNotFoundException($"There is no such series: '{seriesId}'.");
            if (!review.ReviewedSeries.Genres.Any(x => x.Id == genreId)) throw new ResourceNotFoundException($"There is no such genre: '{genreId}'.");

            return review;
        }

        public async Task<IEnumerable<Review>> GetReviewAsync(int genreId, int seriesId)
        {
            var genre = await _context.Genres
                .Include(g => g.Videos)
                    .ThenInclude(s => s.Reviews)
                        .ThenInclude(r => r.Reviewer)
                .SingleOrDefaultAsync(g => g.Id == genreId);
            if (genre == null) throw new ResourceNotFoundException($"There is no such genre: '{genreId}'.");

            var series = genre.Videos.SingleOrDefault(s => s.Id == seriesId);
            if (series == null) throw new ResourceNotFoundException($"There is no such series: '{seriesId}'.");

            return series.Reviews;
        }

        public async Task<bool> DeleteReviewAsync(int genreId, int seriesId, int reviewId)
        {
            var review = await GetReviewAsync(genreId, seriesId, reviewId);

            if (review.Reviewer == null) throw new UnupdateableResourceException($"Reviewer was deleted and the review can no longer be updated.");
            if (review.Reviewer.Id != _userDataProvider.UserId) throw new UnauthorizedException($"Trying to modify review that does not belong to you.");

            _context.Reviews.Remove(review);

            return await SaveAsync();
        }

        public async Task<Review?> InsertReviewAsync(int genreId, int seriesId, Review review)
        {
            if (review.Reviewer == null || review.Reviewer.Id != _userDataProvider.UserId) return null;

            var existingReview = await _context.Reviews.SingleOrDefaultAsync(x => x.Id == review.Id);
            if (existingReview != null) return null;

            var reviewer = await _context.Users.SingleAsync(x => x.Id == _userDataProvider.UserId);
            review.Reviewer = reviewer;

            var series = await _context.Series
                .Include(s => s.Genres)
                .SingleOrDefaultAsync(x => x.Id == seriesId);

            if (series == null) throw new ResourceNotFoundException($"There is no such series: '{seriesId}'.");
            if (!series.Genres.Any(g => g.Id == genreId)) throw new ResourceNotFoundException($"There is no such genre: '{genreId}'.");
            review.ReviewedSeries = series;

            var createdReview = await _context.Reviews.AddAsync(review);
            await SaveAsync();

            return createdReview.Entity;
        }

        public async Task<bool> UpdateReviewAsync(int genreId, int seriesId, int reviewId, Review review)
        {
            var reviewToUpdate = await _context.Reviews
                .Include(r => r.ReviewedSeries)
                    .ThenInclude(s => s.Genres)
                .Include(r => r.Reviewer)
                .SingleOrDefaultAsync(r => r.Id == reviewId);

            if (reviewToUpdate == null) throw new ResourceNotFoundException($"There is no such review: '{reviewId}'.");
            if (reviewToUpdate.Reviewer == null) throw new UnupdateableResourceException($"Reviewer was deleted and the review can no longer be updated.");
            if (reviewToUpdate.Reviewer.Id != _userDataProvider.UserId) throw new UnauthorizedException($"Trying to modify review that does not belong to you.");
            if (reviewToUpdate.ReviewedSeries.Id != seriesId) throw new ResourceNotFoundException("Review series does not match specified series id.");
            if (!reviewToUpdate.ReviewedSeries.Genres.Any(g => g.Id == genreId)) throw new ResourceNotFoundException("Review does not belong in specified genre.");

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
