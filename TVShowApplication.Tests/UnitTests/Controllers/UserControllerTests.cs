using AutoMapper;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TVShowApplication.Controllers;
using TVShowApplication.Data.DTO.Genre;
using TVShowApplication.Data.DTO.Review;
using TVShowApplication.Data.DTO.Series;
using TVShowApplication.Data.DTO.User;
using TVShowApplication.Models;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Tests.UnitTests.Controllers
{
    internal class UserControllerTests
    {
        private Mock<IUserManager> _repository;
        private UserController _testedController;

        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IUserManager>();
            _testedController = new UserController(_repository.Object);
        }

        [Test]
        public async Task SignUp_Succesful()
        {
            var request = new SignUpRequest();
            _repository.Setup(t => t.CreateUser(It.IsAny<SignUpRequest>())).ReturnsAsync(true);

            var result = await _testedController.SignUp(request);

            _repository.Verify(m => m.CreateUser(request), Times.Once());
            Assert.That(result, Is.TypeOf<OkResult>());
        }

        [Test]
        public async Task SignUp_BadRequest()
        {
            var request = new SignUpRequest();
            _repository.Setup(t => t.CreateUser(It.IsAny<SignUpRequest>())).ReturnsAsync(false);

            var result = await _testedController.SignUp(request);

            _repository.Verify(m => m.CreateUser(request), Times.Once());
            Assert.That(result, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        public async Task GetToken_Succesful()
        {
            var request = new SignInRequest();
            var tokens = new AuthenticatedResponse();
            _repository.Setup(t => t.GetTokenForUser(It.IsAny<SignInRequest>())).ReturnsAsync(tokens);

            var result = await _testedController.GetToken(request);

            _repository.Verify(m => m.GetTokenForUser(request), Times.Once());
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            Assert.That((result as OkObjectResult)?.Value, Is.EqualTo(tokens));
        }

        [Test]
        public async Task GetToken_Unauthorized()
        {
            var request = new SignInRequest();
            _repository.Setup(t => t.GetTokenForUser(It.IsAny<SignInRequest>())).ReturnsAsync((AuthenticatedResponse?)null);

            var result = await _testedController.GetToken(request);

            _repository.Verify(m => m.GetTokenForUser(request), Times.Once());
            Assert.That(result, Is.TypeOf<UnauthorizedResult>());
        }

        [Test]
        public async Task RefreshToken_Succesful()
        {
            var request = new RefreshTokenRequest();
            var tokens = new AuthenticatedResponse();
            _repository.Setup(t => t.RefreshToken(It.IsAny<RefreshTokenRequest>())).ReturnsAsync(tokens);

            var result = await _testedController.RefreshToken(request);

            _repository.Verify(m => m.RefreshToken(request), Times.Once());
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            Assert.That((result as OkObjectResult)?.Value, Is.EqualTo(tokens));
        }

        [Test]
        public async Task RefreshToken_Unauthorized()
        {
            var request = new RefreshTokenRequest();
            _repository.Setup(t => t.RefreshToken(It.IsAny<RefreshTokenRequest>())).ReturnsAsync((AuthenticatedResponse?)null);

            var result = await _testedController.RefreshToken(request);

            _repository.Verify(m => m.RefreshToken(request), Times.Once());
            Assert.That(result, Is.TypeOf<UnauthorizedResult>());
        }

        [Test]
        public async Task RevokeToken_Succesful()
        {
            var request = new RefreshTokenRequest();
            var tokens = new AuthenticatedResponse();

            var result = await _testedController.RevokeToken();

            _repository.Verify(m => m.Revoke(null), Times.Once());
            Assert.That(result, Is.TypeOf<OkResult>());
        }

    }
}
