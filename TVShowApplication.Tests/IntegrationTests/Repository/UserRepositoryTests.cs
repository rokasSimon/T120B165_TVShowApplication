using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TVShowApplication.Data;
using TVShowApplication.Exceptions;
using TVShowApplication.Models;
using TVShowApplication.Services.Database;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Tests.IntegrationTests.Repository
{
    internal class UserRepositoryTests
    {
        private const int InvalidId = 9999;

        private TVShowContext _context;
        private IUserRepository _userRepository;

        private User? _basicUser;
        private Poster? _posterUser;
        private Administrator? _adminUser;

        [OneTimeSetUp]
        public void Setup()
        {
            var contextOptions = new DbContextOptionsBuilder<TVShowContext>()
                .UseInMemoryDatabase("TVShowApplication")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new TVShowContext(contextOptions);
            _context.Database.EnsureDeleted();
            _context.SaveChanges();

            _userRepository = new UserRepository(_context);
        }

        [Test, Order(1)]
        public async Task InsertUserAsync_InsertBasicUser_ReturnsCreatedUser()
        {
            var userToCreate = new User()
            {
                Id = 1,
                Email = "rokas.rokanasbasic@gmail.com",
                RefreshToken = null,
                RefreshTokenExpiryTime = DateTime.UtcNow,
                Reviews = new List<Review>(),
                HashedPassword = "password",
                Salt = "salt",
            };

            _basicUser = await _userRepository.InsertUserAsync(userToCreate);

            _basicUser.Should().NotBeNull();
            _basicUser.Should().BeAssignableTo<User>();
        }

        [Test, Order(2)]
        public async Task InsertUserAsync_TakenUserId_ReturnsNull()
        {
            var userToCreate = new User()
            {
                Id = 1,
                Email = "rokas.rokanasbasic@gmail.com",
                RefreshToken = null,
                RefreshTokenExpiryTime = DateTime.UtcNow,
                Reviews = new List<Review>(),
                HashedPassword = "password",
                Salt = "salt",
            };

            var result = await _userRepository.InsertUserAsync(userToCreate);

            result.Should().BeNull();
        }

        [Test, Order(2)]
        public async Task InsertUserAsync_InsertPosterUser_ReturnsCreatedPoster()
        {
            var userToCreate = new Poster()
            {
                Id = 2,
                Email = "rokas.rokanasposter@gmail.com",
                RefreshToken = null,
                RefreshTokenExpiryTime = DateTime.UtcNow,
                Reviews = new List<Review>(),
                HashedPassword = "password",
                Salt = "salt",
                PostedSeries = new List<Series>(),
            };

            _posterUser = await _userRepository.InsertUserAsync(userToCreate);

            _posterUser.Should().NotBeNull();
            _posterUser.Should().BeAssignableTo<Poster>();
        }

        [Test, Order(3)]
        public async Task InsertUserAsync_InsertAdminUser_ReturnsCreatedAdmin()
        {
            var userToCreate = new Administrator()
            {
                Id = 3,
                Email = "rokas.rokanasadmin@gmail.com",
                RefreshToken = null,
                RefreshTokenExpiryTime = DateTime.UtcNow,
                Reviews = new List<Review>(),
                HashedPassword = "password",
                Salt = "salt",
                PostedSeries = new List<Series>(),
            };

            _adminUser = await _userRepository.InsertUserAsync(userToCreate);

            _adminUser.Should().NotBeNull();
            _adminUser.Should().BeAssignableTo<Administrator>();
        }

        [Test, Order(4)]
        public async Task FindUserAsync_GivenValidUserEmail_ReturnsUser()
        {
            var email = _basicUser!.Email;

            var user = await _userRepository.FindUserAsync(email);

            user.Should().NotBeNull();
            user.Should().BeEquivalentTo(_basicUser);
        }

        [Test, Order(4)]
        public async Task FindUserAsync_GivenValidPosterEmail_ReturnsPoster()
        {
            var email = _posterUser!.Email;

            var user = await _userRepository.FindUserAsync(email);

            user.Should().NotBeNull();
            user.Should().BeEquivalentTo(_posterUser);
        }

        [Test, Order(4)]
        public async Task FindUserAsync_GivenValidAdminEmail_ReturnsAdmin()
        {
            var email = _adminUser!.Email;

            var user = await _userRepository.FindUserAsync(email);

            user.Should().NotBeNull();
            user.Should().BeEquivalentTo(_adminUser);
        }

        [Test, Order(4)]
        public async Task GetUserAsync_ValidBasicUser_ReturnsExpectedUser()
        {
            var userToGet = _basicUser!.Id;

            var user = await _userRepository.GetUserAsync<User>(userToGet);

            user.Should().NotBeNull();
            user.Should().BeEquivalentTo(_basicUser);
        }

        [Test, Order(4)]
        public async Task GetUserAsync_ValidPosterUser_ReturnsExpectedPoster()
        {
            var userToGet = _posterUser!.Id;

            var user = await _userRepository.GetUserAsync<Poster>(userToGet);

            user.Should().NotBeNull();
            user.Should().BeEquivalentTo(_posterUser);
        }

        [Test, Order(4)]
        public async Task GetUserAsync_ValidAdminUser_ReturnsExpectedAdmin()
        {
            var userToGet = _adminUser!.Id;

            var user = await _userRepository.GetUserAsync<Administrator>(userToGet);

            user.Should().NotBeNull();
            user.Should().BeEquivalentTo(_adminUser);
        }

        [Test, Order(4)]
        public async Task GetUsersAsync_GetBasicUsers_ReturnsExpectedUsers()
        {
            var users = await _userRepository.GetUsersAsync<User>();

            users.Should().NotBeNull();
            users.Should().BeEquivalentTo(new[] { _basicUser, _posterUser, _adminUser });
        }

        [Test, Order(4)]
        public async Task GetUsersAsync_GetPosterUsers_ReturnsExpectedPosters()
        {
            var users = await _userRepository.GetUsersAsync<Poster>();

            users.Should().NotBeNull();
            users.Should().BeEquivalentTo(new[] { _posterUser, _adminUser });
        }

        [Test, Order(4)]
        public async Task GetUsersAsync_GetAdminUsers_ReturnsExpectedAdmins()
        {
            var users = await _userRepository.GetUsersAsync<Administrator>();

            users.Should().NotBeNull();
            users.Should().BeEquivalentTo(new[] { _adminUser });
        }

        [Test, Order(5)]
        public async Task UpdateUserAsync_GivenValidUserUpdate_ReturnsSuccess()
        {
            const string newEmail = "rokas.rokanasbasic1@gmail.com";
            var userUpdate = new User
            {
                Email = newEmail,
            };

            var success = await _userRepository.UpdateUserAsync(_basicUser!.Id, userUpdate);

            success.Should().BeTrue();
            _basicUser.Email.Should().Be(newEmail);
        }

        [Test, Order(6)]
        public async Task UpdateUserAsync_GivenValidPosterUpdate_ReturnsSuccess()
        {
            const string newEmail = "rokas.rokanasposter1@gmail.com";
            var userUpdate = new User
            {
                Email = newEmail,
            };

            var success = await _userRepository.UpdateUserAsync(_posterUser!.Id, userUpdate);

            success.Should().BeTrue();
            _posterUser.Email.Should().Be(newEmail);
        }

        [Test, Order(7)]
        public async Task UpdateUserAsync_GivenValidAdminUpdate_ReturnsSuccess()
        {
            const string newEmail = "rokas.rokanasadmin1@gmail.com";
            var userUpdate = new User
            {
                Email = newEmail,
            };

            var success = await _userRepository.UpdateUserAsync(_adminUser!.Id, userUpdate);

            success.Should().BeTrue();
            _adminUser.Email.Should().Be(newEmail);
        }

        [Test, Order(8)]
        public async Task DeleteUserAsync_GivenValidUserId_ReturnsSuccess()
        {
            var userToDeleteId = _basicUser!.Id;

            var success = await _userRepository.DeleteUserAsync<User>(userToDeleteId);

            success.Should().BeTrue();
        }

        [Test, Order(9)]
        public async Task DeleteUserAsync_GivenValidPosterId_ReturnsSuccess()
        {
            var userToDeleteId = _posterUser!.Id;

            var success = await _userRepository.DeleteUserAsync<Poster>(userToDeleteId);

            success.Should().BeTrue();
        }

        [Test, Order(10)]
        public async Task DeleteUserAsync_GivenValidAdminId_ReturnsSuccess()
        {
            var userToDeleteId = _adminUser!.Id;

            var success = await _userRepository.DeleteUserAsync<Administrator>(userToDeleteId);

            success.Should().BeTrue();
        }

        [Test, Order(11)]
        public async Task GetUserAsync_GivenInvalidUserId_ReturnsNull()
        {
            var user = await _userRepository.GetUserAsync<User>(InvalidId);

            user.Should().BeNull();
        }

        [Test, Order(11)]
        public async Task FindUserAsync_GivenInvalidUserEmail_ReturnsNull()
        {
            const string validButNotInDbEmail = "rokas.r@gmail.com";

            var user = await _userRepository.FindUserAsync(validButNotInDbEmail);

            user.Should().BeNull();
        }

        [Test, Order(12)]
        public async Task UpdateUserAsync_GivenInvalidUserId_ReturnsFalse()
        {
            const string newEmail = "New Name";
            var oldEmail = _basicUser!.Email;
            var userUpdate = new User
            {
                Email = newEmail,
            };

            var act = () => _userRepository.UpdateUserAsync(InvalidId, userUpdate);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
            _basicUser.Email.Should().Be(oldEmail);
        }

        [Test, Order(13)]
        public async Task DeleteUserAsync_GivenInvalidId_ThrowsNotFoundException()
        {
            var userToDeleteId = InvalidId;

            var act = () => _userRepository.DeleteUserAsync<User>(userToDeleteId);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
