using AutoMapper;
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
using TVShowApplication.Models;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Tests.UnitTests.Controllers
{
    internal class ReviewControllerTests
    {
        private Mock<IReviewRepository> _repository;
        private Mock<IMapper> _mapper;
        private ReviewController _testedController;

        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IReviewRepository>();
            _mapper = new Mock<IMapper>();
            _testedController = new ReviewController(_repository.Object, _mapper.Object);
        }

        [Test]
        public async Task GetReviews_Succesful()
        {
            var genreId = 0;
            var seriesId = 1;
            var review = new Review();
            var reviewDto = new GetReviewDto();
            var reviewDtoList = new List<GetReviewDto>();
            var reviewList = new List<Review>();
            reviewList.Add(review);
            reviewDtoList.Add(reviewDto);
            _repository.Setup(t => t.GetReviewAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(reviewList);
            _mapper.Setup(t => t.Map<IEnumerable<GetReviewDto>>(It.IsAny<IEnumerable<Review>>())).Returns(reviewDtoList);

            var result = await _testedController.GetReviews(genreId, seriesId);

            _repository.Verify(m => m.GetReviewAsync(genreId, seriesId), Times.Once());
            _mapper.Verify(m => m.Map<IEnumerable<GetReviewDto>>(reviewList), Times.Once());
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            Assert.That((result as OkObjectResult)?.Value, Is.EqualTo(reviewDtoList));
        }

        [Test]
        public async Task GetReviews_NotFound()
        {
            var genreId = 0;
            var seriesId = 1;
            var review = new Review();
            var reviewDto = new GetReviewDto();
            var reviewDtoList = new List<GetReviewDto>();
            var reviewList = new List<Review>();
            reviewList.Add(review);
            reviewDtoList.Add(reviewDto);
            _repository.Setup(t => t.GetReviewAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((IEnumerable<Review>)null);
            _mapper.Setup(t => t.Map<IEnumerable<GetReviewDto>>(It.IsAny<IEnumerable<Review>>())).Returns(reviewDtoList);

            var result = await _testedController.GetReviews(genreId, seriesId);

            _repository.Verify(m => m.GetReviewAsync(genreId, seriesId), Times.Once());
            _mapper.Verify(m => m.Map<IEnumerable<GetReviewDto>>(reviewList), Times.Never());
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task GetReviewById_Successful()
        {
            int genreId = 0;
            int seriesId = 1;
            int reviewId = 2;
            var review = new Review();
            var reviewDto = new GetReviewDto();
            _repository.Setup(t => t.GetReviewAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(review);
            _mapper.Setup(t => t.Map<GetReviewDto>(It.IsAny<Review>())).Returns(reviewDto);

            var result = await _testedController.GetReviewById(genreId, seriesId, reviewId);

            _repository.Verify(m => m.GetReviewAsync(genreId, seriesId, reviewId), Times.Once());
            _mapper.Verify(m => m.Map<GetReviewDto>(review), Times.Once());
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            Assert.That((result as OkObjectResult)?.Value, Is.EqualTo(reviewDto));
        }

        [Test]
        public async Task GetReviewById_NotFound()
        {
            int genreId = 0;
            int seriesId = 1;
            int reviewId = 2;
            var review = new Review();
            var reviewDto = new GetReviewDto();
            _repository.Setup(t => t.GetReviewAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((Review?)null);
            _mapper.Setup(t => t.Map<GetReviewDto>(It.IsAny<Review>())).Returns(reviewDto);

            var result = await _testedController.GetReviewById(genreId, seriesId, reviewId);

            _repository.Verify(m => m.GetReviewAsync(genreId, seriesId, reviewId), Times.Once());
            _mapper.Verify(m => m.Map<GetReviewDto>(review), Times.Never());
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task CreateReview_Succesful()
        {
            var genreId = 0;
            var seriesId = 1;
            var createReviewDto = new CreateReviewDto();
            var review = new Review();
            _repository.Setup(t => t.InsertReviewAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Review>())).ReturnsAsync(review);
            _mapper.Setup(t => t.Map<Review>(It.IsAny<CreateReviewDto>())).Returns(review);

            var result = await _testedController.CreateReview(genreId, seriesId, createReviewDto);

            _repository.Verify(m => m.InsertReviewAsync(genreId, seriesId, review), Times.Once());
            _mapper.Verify(m => m.Map<Review>(createReviewDto), Times.Once());
            Assert.That(result, Is.TypeOf<CreatedAtActionResult>());
        }

        [Test]
        public async Task CreateReview_BadRequest()
        {
            var genreId = 0;
            var seriesId = 1;
            var createReviewDto = new CreateReviewDto();
            var review = new Review();
            _repository.Setup(t => t.InsertReviewAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Review>())).ReturnsAsync((Review?)null);
            _mapper.Setup(t => t.Map<Review>(It.IsAny<CreateReviewDto>())).Returns(review);

            var result = await _testedController.CreateReview(genreId, seriesId, createReviewDto);

            _repository.Verify(m => m.InsertReviewAsync(genreId, seriesId, review), Times.Once());
            _mapper.Verify(m => m.Map<Review>(createReviewDto), Times.Once());
            Assert.That(result, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        public async Task UpdateReview_Succesful()
        {
            var genreId = 0;
            var seriesId = 1;
            var reviewId = 2;
            var updateReviewDto = new UpdateReviewDto();
            var review = new Review();
            _repository.Setup(t => t.UpdateReviewAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Review>())).ReturnsAsync(true);
            _mapper.Setup(t => t.Map<Review>(It.IsAny<UpdateReviewDto>())).Returns(review);

            var result = await _testedController.UpdateReview(genreId, seriesId, reviewId, updateReviewDto);

            _repository.Verify(m => m.UpdateReviewAsync(genreId, seriesId,reviewId, review), Times.Once());
            _mapper.Verify(m => m.Map<Review>(updateReviewDto), Times.Once());
            Assert.That(result, Is.TypeOf<OkResult>());
        }

        [Test]
        public async Task UpdateReview_NotFound()
        {
            var genreId = 0;
            var seriesId = 1;
            var reviewId = 2;
            var updateReviewDto = new UpdateReviewDto();
            var review = new Review();
            _repository.Setup(t => t.UpdateReviewAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Review>())).ReturnsAsync(false);
            _mapper.Setup(t => t.Map<Review>(It.IsAny<UpdateReviewDto>())).Returns(review);

            var result = await _testedController.UpdateReview(genreId, seriesId, reviewId, updateReviewDto);

            _repository.Verify(m => m.UpdateReviewAsync(genreId, seriesId, reviewId, review), Times.Once());
            _mapper.Verify(m => m.Map<Review>(updateReviewDto), Times.Once());
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteReview_Succesful()
        {
            var genreId = 0;
            var seriesId = 1;
            var reviewId = 2;
            _repository.Setup(t => t.DeleteReviewAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            var result = await _testedController.DeleteReview(genreId, seriesId, reviewId);

            _repository.Verify(m => m.DeleteReviewAsync(genreId, seriesId, reviewId), Times.Once());
            Assert.That(result, Is.TypeOf<OkResult>());
        }

        [Test]
        public async Task DeleteReview_NotFound()
        {
            var genreId = 0;
            var seriesId = 1;
            var reviewId = 2;
            _repository.Setup(t => t.DeleteReviewAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

            var result = await _testedController.DeleteReview(genreId, seriesId, reviewId);

            _repository.Verify(m => m.DeleteReviewAsync(genreId, seriesId, reviewId), Times.Once());
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }
    }
}
