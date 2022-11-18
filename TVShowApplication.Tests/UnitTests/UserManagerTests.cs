using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TVShowApplication.Data.Options;
using TVShowApplication.Services.Authentication;
using TVShowApplication.Services.Interfaces;
using FluentAssertions;
using Moq;
using TVShowApplication.Data.DTO.User;
using TVShowApplication.Models;
using System.Security.Claims;
using TVShowApplication.Exceptions;

namespace TVShowApplication.Tests.UnitTests
{
    internal class UserManagerTests
    {
        private const string BasicUserSecret = "basic-user";
        private const string PosterUserSecret = "poster-user";
        private const string AdminUserSecret = "admin-user";

        private Mock<IUserDataProvider> _userDataProviderMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IPasswordHasher> _passwordHasherMock;
        private Mock<IJwtGenerator> _jwtGeneratorMock;
        private JwtOptions _jwtOptions;

        private IUserManager _userManager;

        [OneTimeSetUp]
        public void Setup()
        {
            _userDataProviderMock = new Mock<IUserDataProvider>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _jwtGeneratorMock = new Mock<IJwtGenerator>();
            _jwtOptions = new JwtOptions
            {
                RefreshTokenExpirationDays = 1
            };

            _userManager = new UserManager(
                _userDataProviderMock.Object,
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _jwtGeneratorMock.Object,
                Options.Create(_jwtOptions)
            );
        }

        [Test]
        public async Task CreateUser_GivenUntakenEmail_ReturnsSuccess()
        {
            var request = new SignUpRequest
            {
                Email = It.IsAny<string>(),
                Password = It.IsAny<string>(),
                RoleSecret = BasicUserSecret,
            };
            var user = new User();
            _userRepositoryMock.Setup(x => x.FindUserAsync(request.Email)).ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(x => x.InsertUserAsync(It.IsAny<User>())).ReturnsAsync(user);

            var success = await _userManager.CreateUser(request);

            success.Should().Be(true);
        }

        [Test]
        public async Task CreateUser_GivenUnknownSecret_CreatesBasicUserAndReturnsSuccess()
        {
            var request = new SignUpRequest
            {
                Email = It.IsAny<string>(),
                Password = It.IsAny<string>(),
                RoleSecret = It.IsAny<string>(),
            };
            var user = new User();
            _userRepositoryMock.Setup(x => x.FindUserAsync(request.Email)).ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(x => x.InsertUserAsync(It.IsAny<User>())).ReturnsAsync(user);

            var success = await _userManager.CreateUser(request);

            success.Should().Be(true);
        }

        [Test]
        public async Task CreateUser_GivenPosterSecret_CreatesPosterUserAndReturnsSuccess()
        {
            var request = new SignUpRequest
            {
                Email = It.IsAny<string>(),
                Password = It.IsAny<string>(),
                RoleSecret = PosterUserSecret,
            };
            var user = new Poster();
            _userRepositoryMock.Setup(x => x.FindUserAsync(request.Email)).ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(x => x.InsertUserAsync<User>(It.IsAny<Poster>())).ReturnsAsync(user);

            var success = await _userManager.CreateUser(request);

            success.Should().Be(true);
        }

        [Test]
        public async Task CreateUser_GivenAdminSecret_CreatesAdminUserAndReturnsSuccess()
        {
            var request = new SignUpRequest
            {
                Email = It.IsAny<string>(),
                Password = It.IsAny<string>(),
                RoleSecret = AdminUserSecret,
            };
            var user = new Administrator();
            _userRepositoryMock.Setup(x => x.FindUserAsync(request.Email)).ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(x => x.InsertUserAsync<User>(It.IsAny<Administrator>())).ReturnsAsync(user);

            var success = await _userManager.CreateUser(request);

            success.Should().Be(true);
        }

        [Test]
        public async Task CreateUser_GivenTakenEmail_ReturnsFalse()
        {
            var request = new SignUpRequest
            {
                Email = It.IsAny<string>(),
                Password = It.IsAny<string>(),
                RoleSecret = BasicUserSecret,
            };
            var user = new User();
            _userRepositoryMock.Setup(x => x.FindUserAsync(request.Email)).ReturnsAsync(user);

            var success = await _userManager.CreateUser(request);

            success.Should().Be(false);
        }

        [Test]
        public async Task GetTokenForUser_GivenUnknownEmail_ReturnsNull()
        {
            var request = new SignInRequest
            {
                Email = It.IsAny<string>(),
                Password = It.IsAny<string>(),
            };
            var user = new User();
            _userRepositoryMock.Setup(x => x.FindUserAsync(request.Email)).ReturnsAsync((User?)null);

            var tokens = await _userManager.GetTokenForUser(request);

            tokens.Should().BeNull();
        }

        [Test]
        public async Task GetTokenForUser_GivenWrongPassword_ReturnsNull()
        {
            var request = new SignInRequest
            {
                Email = It.IsAny<string>(),
                Password = It.IsAny<string>(),
            };
            var user = new User();
            _userRepositoryMock.Setup(x => x.FindUserAsync(request.Email)).ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.VerifyPassword(user.HashedPassword, request.Password, user.Salt)).Returns(false);

            var tokens = await _userManager.GetTokenForUser(request);

            tokens.Should().BeNull();
            
        }

        [Test]
        public async Task GetTokenForUser_GivenValidEmailAndPassword_ReturnsTokens()
        {
            var request = new SignInRequest
            {
                Email = It.IsAny<string>(),
                Password = It.IsAny<string>(),
            };
            var user = new User();
            var refreshToken = It.IsAny<string>();
            var accessToken = It.IsAny<string>();
            _userRepositoryMock.Setup(x => x.FindUserAsync(request.Email)).ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.VerifyPassword(user.HashedPassword, request.Password, user.Salt)).Returns(true);
            _jwtGeneratorMock.Setup(x => x.GenerateRefreshToken()).Returns(refreshToken);
            _jwtGeneratorMock.Setup(x => x.GenerateToken(It.IsAny<IDictionary<string, string>>())).Returns(accessToken);

            var tokens = await _userManager.GetTokenForUser(request);

            tokens.Should().NotBeNull();
            tokens!.RefreshToken.Should().Be(refreshToken);
            tokens!.AccessToken.Should().Be(accessToken);
            user.RefreshToken.Should().Be(refreshToken);
        }

        [Test]
        public async Task RefreshToken_GivenValidUserAndTokens_ReturnsTokens()
        {
            var request = new RefreshTokenRequest
            {
                AccessToken = It.IsAny<string>(),
                RefreshToken = It.IsAny<string>(),
            };
            var user = new User
            {
                RefreshToken = It.IsAny<string>(),
                RefreshTokenExpiryTime = DateTime.Now.AddMonths(1),
            };
            var refreshToken = It.IsAny<string>();
            var accessToken = It.IsAny<string>();
            var principalMock = new Mock<ClaimsPrincipal>();
            principalMock.Setup(x => x.Claims).Returns(new[] { new Claim(ClaimTypes.NameIdentifier, "1") });
            _userRepositoryMock.Setup(x => x.GetUserAsync<User>(It.IsAny<int>())).ReturnsAsync(user);
            _jwtGeneratorMock.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(principalMock.Object);
            _jwtGeneratorMock.Setup(x => x.GenerateRefreshToken()).Returns(refreshToken);
            _jwtGeneratorMock.Setup(x => x.GenerateToken(It.IsAny<IDictionary<string, string>>())).Returns(accessToken);

            var tokens = await _userManager.RefreshToken(request);

            tokens.Should().NotBeNull();
            tokens!.RefreshToken.Should().Be(refreshToken);
            tokens!.AccessToken.Should().Be(accessToken);
            user.RefreshToken.Should().Be(refreshToken);
        }

        [Test]
        public async Task RefreshToken_GivenUnknownUser_ReturnsNull()
        {
            var request = new RefreshTokenRequest
            {
                AccessToken = It.IsAny<string>(),
                RefreshToken = It.IsAny<string>(),
            };
            var principalMock = new Mock<ClaimsPrincipal>();
            principalMock.Setup(x => x.Claims).Returns(new[] { new Claim(ClaimTypes.NameIdentifier, "1") });
            _jwtGeneratorMock.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(principalMock.Object);
            _userRepositoryMock.Setup(x => x.GetUserAsync<User>(It.IsAny<int>())).ReturnsAsync((User?)null);

            var tokens = await _userManager.RefreshToken(request);

            tokens.Should().BeNull();
        }

        [Test]
        public async Task RefreshToken_GivenExpiredRefreshToken_ReturnsNull()
        {
            var request = new RefreshTokenRequest
            {
                AccessToken = It.IsAny<string>(),
                RefreshToken = It.IsAny<string>(),
            };
            var user = new User
            {
                RefreshToken = It.IsAny<string>(),
                RefreshTokenExpiryTime = DateTime.Now.AddMonths(-1),
            };
            var refreshToken = It.IsAny<string>();
            var accessToken = It.IsAny<string>();
            var principalMock = new Mock<ClaimsPrincipal>();
            principalMock.Setup(x => x.Claims).Returns(new[] { new Claim(ClaimTypes.NameIdentifier, "1") });
            _userRepositoryMock.Setup(x => x.GetUserAsync<User>(It.IsAny<int>())).ReturnsAsync(user);
            _jwtGeneratorMock.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(principalMock.Object);

            var tokens = await _userManager.RefreshToken(request);

            tokens.Should().BeNull();
        }

        [Test]
        public async Task Revoke_DirectlyGivenValidUserId_DoesNotThrow()
        {
            var userId = It.IsAny<int>();
            var user = new User
            {
                RefreshToken = "token",
            };
            _userRepositoryMock.Setup(x => x.GetUserAsync<User>(userId)).ReturnsAsync(user);

            var act = () => _userManager.Revoke(userId);

            await act.Should().NotThrowAsync();
            user.RefreshToken.Should().BeNull();
        }

        [Test]
        public async Task Revoke_DirectlyGivenInvalidUserId_ThrowsResourceNotFound()
        {
            var userId = It.IsAny<int>();
            var user = new User
            {
                RefreshToken = "token",
            };
            _userRepositoryMock.Setup(x => x.GetUserAsync<User>(userId)).ReturnsAsync((User?)null);

            var act = () => _userManager.Revoke(userId);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
            user.RefreshToken.Should().NotBeNull();
        }

        [Test]
        public async Task Revoke_IndirectlyGivenValidUserId_DoesNotThrow()
        {
            var userId = It.IsAny<int>();
            var user = new User
            {
                RefreshToken = "token",
            };
            _userRepositoryMock.Setup(x => x.GetUserAsync<User>(userId)).ReturnsAsync(user);
            _userDataProviderMock.Setup(x => x.UserId).Returns(userId);

            var act = () => _userManager.Revoke();

            await act.Should().NotThrowAsync();
            user.RefreshToken.Should().BeNull();
        }

        [Test]
        public async Task Revoke_IndirectlyGivenInvalidUserId_ThrowsUnauthenticated()
        {
            var userId = It.IsAny<int>();
            var user = new User
            {
                RefreshToken = "token",
            };
            _userRepositoryMock.Setup(x => x.GetUserAsync<User>(userId)).ReturnsAsync((User?)null);
            _userDataProviderMock.Setup(x => x.UserId).Returns(userId);

            var act = () => _userManager.Revoke();

            await act.Should().ThrowAsync<UnauthenticatedException>();
            user.RefreshToken.Should().NotBeNull();
        }
    }
}
