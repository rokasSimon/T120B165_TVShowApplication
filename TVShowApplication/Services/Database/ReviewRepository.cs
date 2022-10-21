using Microsoft.EntityFrameworkCore;
using TVShowApplication.Data;
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

        public async Task<Review?> GetReviewAsync(int genreId, int seriesId, int reviewId)
        {
            //var genre = await _context.Genres
            //    .Include(g => g.Videos)
            //        .ThenInclude(s => s.Reviews)
            //            .ThenInclude(r => r.ReviewedSeries)
            //    .Include(g => g.Videos)
            //        .ThenInclude(s => s.Reviews)
            //            .ThenInclude(r => r.Reviewer)
            //    .Where(x => x.Id == genreId)
            //    .Where(x => x.Videos.Single(x => x.Id == seriesId) == )
            //    .SingleOrDefaultAsync(x => x.Id == genreId);
            //if (genre == null) return null;

            var review = await _context.Reviews
                .Include(r => r.ReviewedSeries)
                    .ThenInclude(s => s.Genres)
                .Include(r => r.Reviewer)
                .SingleOrDefaultAsync(x => x.Id == reviewId);
            if (review == null ||
                review.ReviewedSeries.Id != seriesId ||
                !review.ReviewedSeries.Genres.Any(x => x.Id == genreId)) return null;

            return review;
        }

        public async Task<IEnumerable<Review>> GetReviewAsync(int genreId, int seriesId)
        {
            var genre = await _context.Genres
                .Include(g => g.Videos)
                    .ThenInclude(s => s.Reviews)
                        .ThenInclude(r => r.Reviewer)
                .SingleOrDefaultAsync(g => g.Id == genreId);
            if (genre == null) return Array.Empty<Review>();

            var series = genre.Videos.SingleOrDefault(s => s.Id == seriesId);
            if (series == null) return Array.Empty<Review>();

            return series.Reviews;
        }

        public async Task<bool> DeleteReviewAsync(int genreId, int seriesId, int reviewId)
        {
            var review = await GetReviewAsync(genreId, seriesId, reviewId);
            if (review == null) return false;

            _context.Reviews.Remove(review);

            return await SaveAsync();
        }

        public async Task<Review?> InsertReviewAsync(int genreId, int seriesId, Review review)
        {
            if (review.Reviewer == null || review.Reviewer.Id != _userDataProvider.UserId) return null;

            var reviewer = await _context.Users.SingleOrDefaultAsync(x => x.Id == _userDataProvider.UserId);
            if (reviewer == null) return null;
            review.Reviewer = reviewer;

            var series = await _context.Series
                .Include(s => s.Genres)
                .SingleOrDefaultAsync(x => x.Id == seriesId);
            if (series == null || !series.Genres.Any(g => g.Id == genreId)) return null;
            review.ReviewedSeries = series;

            var createdReview = await _context.Reviews.AddAsync(review);
            var successfullyCreated = await SaveAsync();

            if (successfullyCreated)
            {
                return createdReview.Entity;
            }

            return null;
        }

        public async Task<bool> UpdateReviewAsync(int genreId, int seriesId, int reviewId, Review review)
        {
            var reviewToUpdate = await _context.Reviews
                .Include(r => r.ReviewedSeries)
                    .ThenInclude(s => s.Genres)
                .Include(r => r.Reviewer)
                .SingleOrDefaultAsync(r => r.Id == reviewId);

            if (reviewToUpdate == null ||
                reviewToUpdate.Reviewer == null ||
                reviewToUpdate.Reviewer.Id != _userDataProvider.UserId ||
                reviewToUpdate.ReviewedSeries.Id != seriesId ||
                !reviewToUpdate.ReviewedSeries.Genres.Any(g => g.Id == genreId)) return false;

            reviewToUpdate.Text = review.Text;
            reviewToUpdate.Rating = review.Rating;

            _context.Reviews.Update(reviewToUpdate);

            return await SaveAsync();
        }

        //public async Task<Review?> GetReviewAsync(int id)
        //{
        //    return await _context.Reviews
        //        .Include(r => r.Reviewer)
        //        .Include(r => r.ReviewedSeries)
        //        .SingleOrDefaultAsync(x => x.Id == id);
        //}

        //public async Task<IEnumerable<Review>> GetReviewAsync()
        //{
        //    return await _context.Reviews
        //        .Include(r => r.Reviewer)
        //        .Include(r => r.ReviewedSeries)
        //        .ToListAsync();
        //}

        //public async Task<bool> DeleteReviewAsync(int id)
        //{
        //    var reviewToDelete = new Review { Id = id };

        //    _context.Reviews.Remove(reviewToDelete);

        //    return await SaveAsync();
        //}

        //public async Task<Review?> InsertReviewAsync(Review review)
        //{
        //    if (review.Reviewer == null) return null;
        //    var reviewer = await _context.Users.SingleOrDefaultAsync(x => x.Id == review.Reviewer.Id);

        //    if (reviewer == null) return null;
        //    review.Reviewer = reviewer;

        //    var series = await _context.Series.SingleOrDefaultAsync(x => x.Id == review.ReviewedSeries.Id);
        //    if (series == null) return null;

        //    review.ReviewedSeries = series;

        //    var createdReview = await _context.Reviews.AddAsync(review);
        //    var successfullyCreated = await SaveAsync();

        //    if (successfullyCreated)
        //    {
        //        return createdReview.Entity;
        //    }

        //    return null;
        //}

        //public async Task<bool> UpdateReviewAsync(int id, Review review)
        //{
        //    var reviewToUpdate = await _context.Reviews.SingleOrDefaultAsync(x => x.Id == id);
        //    if (reviewToUpdate == null) return false;

        //    reviewToUpdate.Text = review.Text;
        //    reviewToUpdate.Rating = review.Rating;

        //    _context.Reviews.Update(reviewToUpdate);

        //    return await SaveAsync();
        //}

        private async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() != 0;
        }
    }
}
