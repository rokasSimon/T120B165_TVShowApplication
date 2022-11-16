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
    internal class SeriesRepositoryTests
    {
        private const int InvalidId = 9999;

        private TVShowContext _context;
        private ISeriesRepository _seriesRepository;
        private Genre _genre;
        private Administrator _admin;

        private Series? _createdSeries;

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

            _genre = new Genre
            {
                Id = 1,
                Name = "Test",
                Description = ".",
                Videos = new List<Series>(),
            };

            _context.Admins.Add(_admin);
            _context.Genres.Add(_genre);
            _context.SaveChanges();

            _seriesRepository = new SeriesRepository(_context, new UserDataProvider { UserId = _admin.Id, UserRole = Models.Role.Admin });
        }

        [Test, Order(1)]
        public async Task InsertSeriesAsync_ValidGenreAndSeries_ReturnsCreatedSeries()
        {
            var seriesToCreate = new Series
            {
                Id = 1,
                Name = "Test",
                Description = ".",
                CoverImagePath = ".",
                StarringCast = new[] { "c" },
                Directors = new[] { "d" },
                Poster = _admin,
                Reviews = new List<Review>(),
                Genres = new List<Genre> { _genre },
            };

            _createdSeries = await _seriesRepository.InsertSeriesAsync(_genre.Id, seriesToCreate);

            _createdSeries.Should().NotBeNull();
        }

        [Test, Order(2)]
        public async Task InsertSeriesAsync_TakenSeriesId_ReturnsNull()
        {
            var seriesToCreate = new Series
            {
                Id = 1,
                Name = "Test 2",
                Description = ".",
                CoverImagePath = ".",
                StarringCast = new[] { "c" },
                Directors = new[] { "d" },
                Poster = _admin,
                Reviews = new List<Review>(),
                Genres = new List<Genre> { _genre },
            };

            var result = await _seriesRepository.InsertSeriesAsync(_genre.Id, seriesToCreate);

            result.Should().BeNull();
        }

        [Test, Order(2)]
        public async Task InsertSeriesAsync_MultipleGenresNonExistent_ReturnsNull()
        {
            var seriesToCreate = new Series
            {
                Id = 2,
                Name = "Test 2",
                Description = ".",
                CoverImagePath = ".",
                StarringCast = new[] { "c" },
                Directors = new[] { "d" },
                Poster = _admin,
                Reviews = new List<Review>(),
                Genres = new List<Genre>
                {
                    _genre,
                    new Genre { Id = 2, Name = "Test genre 2", Description = "Test desc 2", Videos = new List<Series>() }
                },
            };

            var result = await _seriesRepository.InsertSeriesAsync(_genre.Id, seriesToCreate);

            result.Should().BeNull();
        }

        [Test, Order(2)]
        public async Task InsertSeriesAsync_PosterNonExistent_ReturnsNull()
        {
            var seriesToCreate = new Series
            {
                Id = 2,
                Name = "Test 2",
                Description = ".",
                CoverImagePath = ".",
                StarringCast = new[] { "c" },
                Directors = new[] { "d" },
                Poster = new Poster { Id = InvalidId, Email = "tes.test@gmail.com", HashedPassword = "test", Salt = "test", PostedSeries = new List<Series>(), Reviews = new List<Review>() },
                Reviews = new List<Review>(),
                Genres = new List<Genre>
                {
                    _genre,
                },
            };

            var result = await _seriesRepository.InsertSeriesAsync(_genre.Id, seriesToCreate);

            result.Should().BeNull();
        }

        [Test, Order(3)]
        public async Task GetSeriesAsync_ValidGenreAndSeries_ReturnsExpectedSeries()
        {
            var seriesToGet = _createdSeries!.Id;

            var series = await _seriesRepository.GetSeriesAsync(_genre.Id, seriesToGet);

            series.Should().NotBeNull();
            series.Should().BeEquivalentTo(_createdSeries);
        }

        [Test, Order(3)]
        public async Task GetSeriesAsync_GivenOnlyGenre_ReturnsExpectedSeries()
        {
            var series = await _seriesRepository.GetSeriesAsync(_genre.Id);

            series.Should().NotBeNull();
            series.Should().BeEquivalentTo(new[] { _createdSeries });
        }

        [Test, Order(4)]
        public async Task UpdateSeriesAsync_GivenValidGenreAndSeriesUpdate_ReturnsSuccess()
        {
            const string newDescription = "New Name";
            var seriesToUpdate = new Series
            {
                Description = newDescription,
                Directors = new[] { "Christopher Nolan" },
                StarringCast = new[] { "Leonardo DiCaprio" },
            };

            var success = await _seriesRepository.UpdateSeriesAsync(_genre.Id, _createdSeries!.Id, seriesToUpdate);

            success.Should().BeTrue();
            _createdSeries!.Description.Should().Be(newDescription);
        }

        [Test, Order(5)]
        public async Task DeleteSeriesAsync_GivenValidGenreAndSeries_ReturnsSuccess()
        {
            var seriesToDelete = _createdSeries!.Id;

            var success = await _seriesRepository.DeleteSeriesAsync(_genre.Id, seriesToDelete);

            success.Should().BeTrue();
        }

        [Test, Order(6)]
        public async Task GetSingleSeriesAsync_GivenInvalidGenre_ThrowsNotFound()
        {
            var act = () => _seriesRepository.GetSeriesAsync(InvalidId, _createdSeries!.Id);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Test, Order(7)]
        public async Task GetSeriesAsync_GivenInvalidGenre_ThrowsNotFound()
        {
            var act = () => _seriesRepository.GetSeriesAsync(InvalidId);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Test, Order(8)]
        public async Task GetSingleSeriesAsync_GivenValidGenreAndInvalidSeries_ReturnsNull()
        {
            var series = await _seriesRepository.GetSeriesAsync(_genre.Id, InvalidId);

            series.Should().BeNull();
        }

        [Test, Order(9)]
        public async Task UpdateSeriesAsync_GivenInvalidGenreId_ThrowsNotFound()
        {
            const string newDescription = "New Name";
            var oldDescription = _createdSeries!.Description;
            var seriesToUpdate = new Series
            {
                Description = newDescription,
                Directors = new[] { "Christopher Nolan" },
                StarringCast = new[] { "Leonardo DiCaprio" },
            };

            var act = () => _seriesRepository.UpdateSeriesAsync(9999, seriesToUpdate.Id, seriesToUpdate);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
            _createdSeries.Description.Should().Be(oldDescription);
        }

        [Test, Order(10)]
        public async Task UpdateSeriesAsync_GivenValidGenreAndInvalidSeries_ThrowsNotFound()
        {
            const string newDescription = "New Name";
            var oldDescription = _createdSeries!.Description;
            var seriesToUpdate = new Series
            {
                Description = newDescription,
                Directors = new[] { "Christopher Nolan" },
                StarringCast = new[] { "Leonardo DiCaprio" },
            };

            var act = () => _seriesRepository.UpdateSeriesAsync(_genre.Id, InvalidId, seriesToUpdate);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
            _createdSeries.Description.Should().Be(oldDescription);
        }

        [Test, Order(11)]
        public async Task DeleteSeriesAsync_GivenInvalidGenre_ThrowsNotFoundException()
        {
            var act = () => _seriesRepository.DeleteSeriesAsync(InvalidId, _createdSeries!.Id);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Test, Order(12)]
        public async Task DeleteSeriesAsync_GivenValidGenreAndInvalidSeries_ThrowsNotFoundException()
        {
            var act = () => _seriesRepository.DeleteSeriesAsync(_genre.Id, InvalidId);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
