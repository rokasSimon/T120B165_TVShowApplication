using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Security.Principal;
using TVShowApplication.Exceptions;
using TVShowApplication.Middleware;
using TVShowApplication.Models;
using TVShowApplication.Services.Authentication;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Tests.UnitTests.Middleware
{
    [TestFixture]
    internal class UserDataMiddlewareTests
    {
        private UserDataMiddleware _middleware;

        [OneTimeSetUp]
        public void Setup()
        {
            _middleware = new UserDataMiddleware((next) => { return Task.CompletedTask; });
        }

        [Test]
        public async Task InvokeAsync_WhenUserIsAuthenticatedAndHasClaims_SetsUserData()
        {
            const Role roleValue = Role.Poster;
            const int idValue = 1;
            var context = new Mock<HttpContext>();
            context.Setup(x => x.User.Identity!.IsAuthenticated).Returns(true);
            context.Setup(x => x.User.Claims).Returns(new List<Claim>
            {
                new Claim(ClaimTypes.Role, roleValue.ToString()),
                new Claim(ClaimTypes.NameIdentifier, idValue.ToString()),
            });
            var userDataProvider = new UserDataProvider();

            var act = () => _middleware.InvokeAsync(context.Object, userDataProvider);

            await act.Should().NotThrowAsync();
            userDataProvider.UserId.Should().Be(idValue);
            userDataProvider.UserRole.Should().Be(roleValue);
        }

        [Test]
        public async Task InvokeAsync_WhenUserIsNull_DoesNotThrowException()
        {
            var context = new Mock<HttpContext>();
            context.Setup(x => x.User.Identity).Returns((IIdentity?)null);
            var userDataProvider = Mock.Of<IUserDataProvider>();

            var act = () => _middleware.InvokeAsync(context.Object, userDataProvider);

            await act.Should().NotThrowAsync();
        }

        [Test]
        public async Task InvokeAsync_WhenUserIsNotAuthenticated_DoesNotThrowException()
        {
            var context = new Mock<HttpContext>();
            context.Setup(x => x.User.Identity!.IsAuthenticated).Returns(false);
            var userDataProvider = Mock.Of<IUserDataProvider>();

            var act = () => _middleware.InvokeAsync(context.Object, userDataProvider);

            await act.Should().NotThrowAsync();
        }

        [TestCaseSource(nameof(MissingClaimsLists))]
        public async Task InvokeAsync_WhenClaimsNotGiven_ThrowsUnauthorized(IEnumerable<Claim> givenClaims)
        {
            var context = new Mock<HttpContext>();
            context.Setup(x => x.User.Identity!.IsAuthenticated).Returns(true);
            context.Setup(x => x.User.Claims).Returns(givenClaims);
            var userDataProvider = new UserDataProvider();

            var act = () => _middleware.InvokeAsync(context.Object, userDataProvider);

            await act.Should().ThrowAsync<UnauthorizedException>();
        }

        private static IEnumerable<TestCaseData> MissingClaimsLists()
        {
            yield return new TestCaseData(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, 1.ToString())
            });
            yield return new TestCaseData(new List<Claim>
            {
                new Claim(ClaimTypes.Role, Role.Poster.ToString())
            });
        }
    }
}
