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
    internal class GenreRepositoryTests
    {
        private const int InvalidId = 9999;

        private TVShowContext _context;
        private IUserDataProvider _userDataProvider;
        private IGenreRepository _genreRepository;

        private Genre? _createdGenre;

        [OneTimeSetUp]
        public void Setup()
        {
            var contextOptions = new DbContextOptionsBuilder<TVShowContext>()
                .UseInMemoryDatabase("TVShowApplication")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new TVShowContext(contextOptions);
            _context.Database.EnsureDeleted();

            var adminUser = new Administrator
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

            _context.Admins.Add(adminUser);
            _context.SaveChanges();

            _userDataProvider = new UserDataProvider { UserId = adminUser.Id, UserRole = Models.Role.Admin };
            _genreRepository = new GenreRepository(_context, _userDataProvider);
        }

        [Test, Order(1)]
        public async Task InsertGenreAsync_ValidGenre_ReturnsCreatedGenre()
        {
            var genreToCreate = new Genre { Id = 1, Name = "Test", Description = "Test", Videos = new List<Series>() };

            _createdGenre = await _genreRepository.InsertGenreAsync(genreToCreate);

            _createdGenre.Should().NotBeNull();
        }

        [Test, Order(2)]
        public async Task InsertGenreAsync_TakenGenreId_ReturnsNull()
        {
            var genreToCreate = new Genre { Id = 1, Name = "Test 2", Description = "Test 2", Videos = new List<Series>() };

            var result = await _genreRepository.InsertGenreAsync(genreToCreate);

            result.Should().BeNull();
        }

        [Test, Order(2)]
        public async Task GetGenreAsync_ValidGenre_ReturnsExpectedGenre()
        {
            var genreToGetId = _createdGenre!.Id;

            var genre = await _genreRepository.GetGenreAsync(genreToGetId);

            genre.Should().NotBeNull();
            genre.Should().BeEquivalentTo(_createdGenre);
        }

        [Test, Order(3)]
        public async Task GetGenresAsync_GivenNoParameters_ReturnsExpectedGenres()
        {
            var genres = await _genreRepository.GetGenresAsync();

            genres.Should().NotBeNull();
            genres.Should().BeEquivalentTo(new[] { _createdGenre });
        }

        [Test, Order(4)]
        public async Task UpdateGenreAsync_GivenValidGenreUpdate_ReturnsSuccess()
        {
            const string newDescription = "New Name";
            var genreUpdate = new Genre { Id = _createdGenre!.Id, Name = _createdGenre.Name, Description = newDescription, Videos = _createdGenre.Videos };

            var success = await _genreRepository.UpdateGenreAsync(genreUpdate.Id, genreUpdate);

            success.Should().BeTrue();
            _createdGenre.Description.Should().Be(newDescription);
        }

        [Test, Order(5)]
        public async Task DeleteGenreAsync_GivenValidGenreId_ReturnsSuccess()
        {
            var genreToDeleteId = _createdGenre!.Id;

            var success = await _genreRepository.DeleteGenreAsync(genreToDeleteId);

            success.Should().BeTrue();
        }

        [Test, Order(6)]
        public async Task GetGenreAsync_GivenInvalidGenreId_ReturnsNull()
        {
            var genre = await _genreRepository.GetGenreAsync(_createdGenre!.Id);

            genre.Should().BeNull();
        }

        [Test, Order(7)]
        public async Task UpdateGenreAsync_GivenInvalidGenreId_ReturnsFalse()
        {
            const string newDescription = "New Name";
            var oldDescription = _createdGenre!.Description;
            var genreUpdate = new Genre { Id = InvalidId, Name = _createdGenre.Name, Description = newDescription, Videos = _createdGenre.Videos };

            var success = await _genreRepository.UpdateGenreAsync(genreUpdate.Id, genreUpdate);

            success.Should().BeFalse();
            _createdGenre.Description.Should().Be(oldDescription);
        }

        [Test, Order(8)]
        public async Task DeleteGenreAsync_GivenInvalidId_ThrowsNotFoundException()
        {
            var genreToDeleteId = InvalidId;

            var act = () => _genreRepository.DeleteGenreAsync(genreToDeleteId);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
