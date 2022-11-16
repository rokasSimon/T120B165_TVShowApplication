using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TVShowApplication.Data;
using TVShowApplication.Exceptions;
using TVShowApplication.Models;
using TVShowApplication.Services.Authentication;
using TVShowApplication.Services.Database;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Tests.IntegrationTests.Repository
{
    [TestFixture]
    internal class ReviewRepositoryTests
    {
        private const int InvalidId = 9999;

        private TVShowContext _context;
        private IUserDataProvider _userDataProvider;
        private IReviewRepository _reviewRepository;

        private Genre _genre;
        private Series _series;
        private Administrator _admin;
        private Administrator _secondAdmin;

        private Review? _createdReview;

        [OneTimeSetUp]
        public void Setup()
        {
            var contextOptions = new DbContextOptionsBuilder<TVShowContext>()
                .UseInMemoryDatabase("TVShowApplication")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new TVShowContext(contextOptions);
            _context.Database.EnsureDeleted();

            _admin = new Administrator
            {
                Id = 1,
                Email = "test@gmail.com",
                HashedPassword = "fbaojalfnafljb",
                RefreshToken = "ajfbalfmn",
                RefreshTokenExpiryTime = DateTime.Today.AddDays(2),
                Salt = "ofgahnfa",
                PostedSeries = new List<Series>(),
                Reviews = new List<Review>(),
            };
            _secondAdmin = new Administrator
            {
                Id = 2,
                Email = "test.test@gmail.com",
                HashedPassword = "fbaojalfnafljb",
                RefreshToken = "ajfbalfmn",
                RefreshTokenExpiryTime = DateTime.Today.AddDays(2),
                Salt = "ofgahnfa",
                PostedSeries = new List<Series>(),
                Reviews = new List<Review>(),
            };

            _genre = new Genre
            {
                Id = 1,
                Name = "Test",
                Description = ".",
                Videos = new List<Series>(),
            };

            _series = new Series
            {
                Id = 1,
                Name = "Test Name",
                Description = "Test Description",
                CoverImagePath = "https://test.com",
                Poster = _admin,
                Directors = new List<string> { "Director #1", "Director #2" },
                StarringCast = new List<string> { "Actor #1", "Actor #2" },
                Genres = new List<Genre> { _genre },
                Reviews = new List<Review>(),
            };

            _context.Admins.Add(_admin);
            _context.Admins.Add(_secondAdmin);
            _context.Genres.Add(_genre);
            _context.Series.Add(_series);
            _context.SaveChanges();

            _userDataProvider = new UserDataProvider { UserId = _admin.Id, UserRole = Models.Role.Admin };
            _reviewRepository = new ReviewRepository(_context, _userDataProvider);
        }

        [Test, Order(1)]
        [Category("Happy Path")]
        public async Task InsertReviewAsync_ValidGenreAndSeriesAndReview_ReturnsCreatedReview()
        {
            var reviewToCreate = new Review
            {
                Id = 1,
                Rating = 10,
                Text = "good",
                PostDate = DateTime.Now,
                Reviewer = _admin,
            };

            _createdReview = await _reviewRepository.InsertReviewAsync(_genre.Id, _series.Id, reviewToCreate);

            _createdReview.Should().NotBeNull();
            _createdReview!.Reviewer!.Id.Should().Be(_admin.Id);
        }

        [Test, Order(2)]
        [Category("Error Path")]
        public async Task InsertReviewAsync_TakenReviewId_ReturnsNull()
        {
            var reviewToCreate = new Review
            {
                Id = 1,
                Rating = 10,
                Text = "good",
                PostDate = DateTime.Now,
                Reviewer = _admin,
            };

            var result = await _reviewRepository.InsertReviewAsync(_genre.Id, _series.Id, reviewToCreate);

            result.Should().BeNull();
        }

        [Test, Order(2)]
        [Category("Error Path")]
        public async Task InsertReviewAsync_NullReviewer_ReturnsNull()
        {
            var reviewToCreate = new Review
            {
                Id = 1,
                Rating = 10,
                Text = "good",
                PostDate = DateTime.Now,
                Reviewer = null,
            };

            var result = await _reviewRepository.InsertReviewAsync(_genre.Id, _series.Id, reviewToCreate);

            result.Should().BeNull();
        }

        [Test, Order(2)]
        [Category("Error Path")]
        public async Task InsertReviewAsync_ReviewerIdDoesNotMatchUser_ReturnsNull()
        {
            var reviewToCreate = new Review
            {
                Id = 2,
                Rating = 10,
                Text = "good",
                PostDate = DateTime.Now,
                Reviewer = new User { Id = InvalidId },
            };

            var result = await _reviewRepository.InsertReviewAsync(_genre.Id, _series.Id, reviewToCreate);

            result.Should().BeNull();
        }

        [Test, Order(2)]
        [Category("Error Path")]
        public async Task InsertReviewAsync_SeriesDoesNotExist_ThrowsNotFound()
        {
            var reviewToCreate = new Review
            {
                Id = 2,
                Rating = 10,
                Text = "good",
                PostDate = DateTime.Now,
                Reviewer = _admin,
            };

            var act = () => _reviewRepository.InsertReviewAsync(_genre.Id, InvalidId, reviewToCreate);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Test, Order(3)]
        [Category("Happy Path")]
        public async Task GetReviewAsync_ValidGenreAndSeriesAndReview_ReturnsExpectedReview()
        {
            var reviewToGet = _createdReview!.Id;

            var review = await _reviewRepository.GetReviewAsync(_genre.Id, _series.Id, reviewToGet);

            review.Should().NotBeNull();
            review.Should().BeEquivalentTo(_createdReview);
        }

        [Test, Order(3)]
        [Category("Happy Path")]
        public async Task GetReviewAsync_GivenOnlyGenreAndSeries_ReturnsExpectedSeries()
        {
            var review = await _reviewRepository.GetReviewAsync(_genre.Id, _series.Id);

            review.Should().NotBeNull();
            review.Should().BeEquivalentTo(new[] { _createdReview });
        }

        [Test, Order(4)]
        [Category("Happy Path")]
        public async Task UpdateReviewAsync_GivenValidGenreAndSeriesAndReviewUpdate_ReturnsSuccess()
        {
            const string newText = "Movie sucks";
            var reviewToUpdate = new Review
            {
                Rating = 1,
                Text = newText
            };

            var success = await _reviewRepository.UpdateReviewAsync(_genre.Id, _series!.Id, _createdReview!.Id, reviewToUpdate);

            success.Should().BeTrue();
            _createdReview!.Text.Should().Be(newText);
        }

        [Test, Order(6)]
        [Category("Error Path")]
        public async Task GetSingleReviewAsync_GivenInvalidGenre_ThrowsNotFound()
        {
            var act = () => _reviewRepository.GetReviewAsync(InvalidId, _series.Id, _createdReview!.Id);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Test, Order(7)]
        [Category("Error Path")]
        public async Task GetSingleReviewAsync_GivenInvalidSeries_ThrowsNotFound()
        {
            var act = () => _reviewRepository.GetReviewAsync(_genre.Id, InvalidId, _createdReview!.Id);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Test, Order(8)]
        [Category("Error Path")]
        public async Task GetSingleReviewAsync_GivenInvalidReview_ThrowsNotFound()
        {
            var act = () => _reviewRepository.GetReviewAsync(_genre.Id, _series.Id, InvalidId);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Test, Order(9)]
        [Category("Error Path")]
        public async Task GetReviewAsync_GivenInvalidGenre_ThrowsNotFound()
        {
            var act = () => _reviewRepository.GetReviewAsync(InvalidId, _series.Id);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Test, Order(10)]
        [Category("Error Path")]
        public async Task GetReviewAsync_GivenInvalidSeries_ThrowsNotFound()
        {
            var act = () => _reviewRepository.GetReviewAsync(_genre.Id, InvalidId);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Test, Order(11)]
        [Category("Error Path")]
        public async Task UpdateReviewAsync_GivenInvalidGenreId_ThrowsNotFound()
        {
            const string newText = "New Name";
            var oldText = _createdReview!.Text;
            var reviewToUpdate = new Review
            {
                Text = newText,
            };

            var act = () => _reviewRepository.UpdateReviewAsync(InvalidId, _series.Id, _createdReview.Id, reviewToUpdate);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
            _createdReview.Text.Should().Be(oldText);
        }

        [Test, Order(12)]
        [Category("Error Path")]
        public async Task UpdateReviewAsync_GivenInvalidSeriesId_ThrowsNotFound()
        {
            const string newText = "New Name";
            var oldText = _createdReview!.Text;
            var reviewToUpdate = new Review
            {
                Text = newText,
            };

            var act = () => _reviewRepository.UpdateReviewAsync(_genre.Id, InvalidId, _createdReview.Id, reviewToUpdate);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
            _createdReview.Text.Should().Be(oldText);
        }

        [Test, Order(13)]
        [Category("Error Path")]
        public async Task UpdateReviewAsync_GivenInvalidReviewId_ThrowsNotFound()
        {
            const string newText = "New Name";
            var oldText = _createdReview!.Text;
            var reviewToUpdate = new Review
            {
                Text = newText,
            };

            var act = () => _reviewRepository.UpdateReviewAsync(_genre.Id, _series.Id, InvalidId, reviewToUpdate);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
            _createdReview.Text.Should().Be(oldText);
        }

        [Test, Order(14)]
        [Category("Error Path")]
        public async Task DeleteReviewAsync_GivenInvalidGenre_ThrowsNotFoundException()
        {
            var act = () => _reviewRepository.DeleteReviewAsync(InvalidId, _series!.Id, _createdReview!.Id);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Test, Order(15)]
        [Category("Error Path")]
        public async Task DeleteReviewAsync_GivenInvalidSeries_ThrowsNotFoundException()
        {
            var act = () => _reviewRepository.DeleteReviewAsync(_genre.Id, InvalidId, _createdReview!.Id);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Test, Order(16)]
        [Category("Error Path")]
        public async Task DeleteReviewAsync_GivenInvalidReview_ThrowsNotFoundException()
        {
            var act = () => _reviewRepository.DeleteReviewAsync(_genre.Id, _series.Id, InvalidId);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Test, Order(17)]
        [Category("Error Path")]
        public async Task UpdateReviewAsync_TryUpdatingOtherUserReview_ThrowsUnauthorized()
        {
            _userDataProvider.UserId = _secondAdmin.Id;
            const string newText = "New Name";
            var oldText = _createdReview!.Text;
            var reviewToUpdate = new Review
            {
                Text = newText,
            };

            var act = () => _reviewRepository.UpdateReviewAsync(_genre.Id, _series.Id, _createdReview.Id, reviewToUpdate);

            await act.Should().ThrowAsync<UnauthorizedException>();
            _createdReview.Text.Should().Be(oldText);
        }

        [Test, Order(18)]
        [Category("Error Path")]
        public async Task DeleteReviewAsync_TryDeletingOtherUserReview_ThrowsUnauthorized()
        {
            _userDataProvider.UserId = _secondAdmin.Id;
            const string newText = "New Name";
            var oldText = _createdReview!.Text;
            var reviewToUpdate = new Review
            {
                Text = newText,
            };

            var act = () => _reviewRepository.DeleteReviewAsync(_genre.Id, _series.Id, _createdReview.Id);

            await act.Should().ThrowAsync<UnauthorizedException>();
            _createdReview.Text.Should().Be(oldText);
        }

        [Test, Order(19)]
        [Category("Happy Path")]
        public async Task DeleteReviewAsync_GivenValidGenreAndSeriesAndReview_ReturnsSuccess()
        {
            _userDataProvider.UserId = _admin.Id;
            var reviewToDelete = _createdReview!.Id;

            var success = await _reviewRepository.DeleteReviewAsync(_genre.Id, _series.Id, reviewToDelete);

            success.Should().BeTrue();
        }

        [Test, Order(20)]
        [Category("Happy Path")]
        public async Task InsertReviewAsync_InsertReviewForOtherTests_ReturnsSuccess()
        {
            var reviewToCreate = new Review
            {
                Id = 2,
                Rating = 10,
                Text = "good",
                PostDate = DateTime.Now,
                Reviewer = _admin,
            };

            _createdReview = await _reviewRepository.InsertReviewAsync(_genre.Id, _series.Id, reviewToCreate);
            _context.Admins.Remove(_admin);
            _context.SaveChanges();

            _createdReview.Should().NotBeNull();
            _createdReview!.Reviewer.Should().BeNull();
        }

        [Test, Order(21)]
        [Category("Error Path")]
        public async Task UpdateReviewAsync_UpdateDeletedReviewerReview_ThrowsUnupdateable()
        {
            _userDataProvider.UserId = _secondAdmin.Id;
            const string newText = "New Name";
            var oldText = _createdReview!.Text;
            var reviewToUpdate = new Review
            {
                Text = newText,
            };

            var act = () => _reviewRepository.UpdateReviewAsync(_genre.Id, _series.Id, _createdReview.Id, reviewToUpdate);

            await act.Should().ThrowAsync<UnupdateableResourceException>();
            _createdReview.Text.Should().Be(oldText);
        }

        [Test, Order(22)]
        [Category("Error Path")]
        public async Task DeleteReviewAsync_DeleteDeletedReviewerReview_ThrowsUnupdateable()
        {
            _userDataProvider.UserId = _secondAdmin.Id;
            const string newText = "New Name";
            var oldText = _createdReview!.Text;
            var reviewToUpdate = new Review
            {
                Text = newText,
            };

            var act = () => _reviewRepository.DeleteReviewAsync(_genre.Id, _series.Id, _createdReview.Id);

            await act.Should().ThrowAsync<UnupdateableResourceException>();
            _createdReview.Text.Should().Be(oldText);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
