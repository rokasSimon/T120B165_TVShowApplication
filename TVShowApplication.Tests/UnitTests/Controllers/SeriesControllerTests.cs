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
using TVShowApplication.Data.DTO.Series;
using TVShowApplication.Models;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Tests.UnitTests.Controllers
{
    internal class SeriesControllerTests
    {
        private Mock<ISeriesRepository> _repository;
        private Mock<IMapper> _mapper;
        private SeriesController _testedController;

        [SetUp]
        public void Setup()
        {
            _repository = new Mock<ISeriesRepository>();
            _mapper = new Mock<IMapper>();
            _testedController = new SeriesController(_repository.Object, _mapper.Object);
        }

        [Test]
        public async Task GetSeries_Succesful()
        {
            var genreId = 0;
            var series = new Series();
            var seriesDto = new GetSeriesDto();
            var seriesDtoList = new List<GetSeriesDto>();
            var seriesList = new List<Series>();
            seriesList.Add(series);
            seriesDtoList.Add(seriesDto);
            _repository.Setup(t => t.GetSeriesAsync(It.IsAny<int>())).ReturnsAsync(seriesList);
            _mapper.Setup(t => t.Map<IEnumerable<GetSeriesDto>>(It.IsAny<IEnumerable<Series>>())).Returns(seriesDtoList);

            var result = await _testedController.GetSeries(genreId);

            _repository.Verify(m => m.GetSeriesAsync(genreId), Times.Once());
            _mapper.Verify(m => m.Map<IEnumerable<GetSeriesDto>>(seriesList), Times.Once());
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            Assert.That((result as OkObjectResult)?.Value, Is.EqualTo(seriesDtoList));
        }

        [Test]
        public async Task GetSeries_NotFound()
        {
            var genreId = 0;
            var series = new Series();
            var seriesDto = new GetSeriesDto();
            var seriesDtoList = new List<GetSeriesDto>();
            var seriesList = new List<Series>();
            seriesList.Add(series);
            seriesDtoList.Add(seriesDto);
            _repository.Setup(t => t.GetSeriesAsync(It.IsAny<int>())).ReturnsAsync((IEnumerable<Series>)null);
            _mapper.Setup(t => t.Map<IEnumerable<GetSeriesDto>>(It.IsAny<IEnumerable<Series>>())).Returns(seriesDtoList);

            var result = await _testedController.GetSeries(genreId);

            _repository.Verify(m => m.GetSeriesAsync(genreId), Times.Once());
            _mapper.Verify(m => m.Map<IEnumerable<GetSeriesDto>>(seriesList), Times.Never());
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task GetSeriesById_Sucessful()
        {
            var genreId = 0;
            var seriesId = 1;
            var series = new Series();
            var seriesDto = new GetSeriesDto();
            _repository.Setup(t => t.GetSeriesAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(series);
            _mapper.Setup(t => t.Map<GetSeriesDto>(It.IsAny<Series>())).Returns(seriesDto);

            var result = await _testedController.GetSeriesById(genreId, seriesId);

            _repository.Verify(m => m.GetSeriesAsync(genreId, seriesId), Times.Once());
            _mapper.Verify(m => m.Map<GetSeriesDto>(series), Times.Once());
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            Assert.That((result as OkObjectResult)?.Value, Is.EqualTo(seriesDto));
        }

        [Test]
        public async Task GetSeriesById_NotFound()
        {
            var genreId = 0;
            var seriesId = 1;
            var series = new Series();
            var seriesDto = new GetSeriesDto();
            _repository.Setup(t => t.GetSeriesAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((Series?)null);
            _mapper.Setup(t => t.Map<GetSeriesDto>(It.IsAny<Series>())).Returns(seriesDto);

            var result = await _testedController.GetSeriesById(genreId, seriesId);

            _repository.Verify(m => m.GetSeriesAsync(genreId, seriesId), Times.Once());
            _mapper.Verify(m => m.Map< GetSeriesDto > (series), Times.Never());
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task CreateSeries_Succesful()
        {
            var genreId = 0;
            var createSeriesDto = new CreateSeriesDto();
            var series = new Series();
            _repository.Setup(t => t.InsertSeriesAsync(It.IsAny<int>(), It.IsAny<Series>())).ReturnsAsync(series);
            _mapper.Setup(t => t.Map<Series>(It.IsAny<CreateSeriesDto>())).Returns(series);

            var result = await _testedController.CreateSeries(genreId, createSeriesDto);

            _repository.Verify(m => m.InsertSeriesAsync(genreId, series), Times.Once());
            _mapper.Verify(m => m.Map<Series>(createSeriesDto), Times.Once());
            Assert.That(result, Is.TypeOf<CreatedAtActionResult>());
        }


        [Test]
        public async Task CreateSeries_BadRequest()
        {
            var genreId = 0;
            var createSeriesDto = new CreateSeriesDto();
            var series = new Series();
            _repository.Setup(t => t.InsertSeriesAsync(It.IsAny<int>(), It.IsAny<Series>())).ReturnsAsync((Series?)null);
            _mapper.Setup(t => t.Map<Series>(It.IsAny<CreateSeriesDto>())).Returns(series);

            var result = await _testedController.CreateSeries(genreId, createSeriesDto);

            _repository.Verify(m => m.InsertSeriesAsync(genreId, series), Times.Once());
            _mapper.Verify(m => m.Map<Series>(createSeriesDto), Times.Once());
            Assert.That(result, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        public async Task UpdateSeries_Succesful()
        {
            var genreId = 0;
            var seriesId = 1;
            var updateSeriesDto = new UpdateSeriesDto();
            var series = new Series();
            _repository.Setup(t => t.UpdateSeriesAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Series>())).ReturnsAsync(true);
            _mapper.Setup(t => t.Map<Series>(It.IsAny<UpdateSeriesDto>())).Returns(series);

            var result = await _testedController.UpdateSeries(genreId,seriesId, updateSeriesDto);

            _repository.Verify(m => m.UpdateSeriesAsync(genreId, seriesId, series), Times.Once());
            _mapper.Verify(m => m.Map<Series>(updateSeriesDto), Times.Once());
            Assert.That(result, Is.TypeOf<OkResult>());
        }


        [Test]
        public async Task UpdateSeries_NotFound()
        {
            var genreId = 0;
            var seriesId = 1;
            var updateSeriesDto = new UpdateSeriesDto();
            var series = new Series();
            _repository.Setup(t => t.UpdateSeriesAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Series>())).ReturnsAsync(false);
            _mapper.Setup(t => t.Map<Series>(It.IsAny<UpdateSeriesDto>())).Returns(series);

            var result = await _testedController.UpdateSeries(genreId, seriesId, updateSeriesDto);

            _repository.Verify(m => m.UpdateSeriesAsync(genreId, seriesId, series), Times.Once());
            _mapper.Verify(m => m.Map<Series>(updateSeriesDto), Times.Once());
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteSeries_Succesful()
        {
            var genreId = 0;
            var seriesId = 1;
            _repository.Setup(t => t.DeleteSeriesAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            var result = await _testedController.DeleteSeries(genreId, seriesId);

            _repository.Verify(m => m.DeleteSeriesAsync(genreId, seriesId), Times.Once());
            Assert.That(result, Is.TypeOf<OkResult>());
        }

        [Test]
        public async Task DeleteSeries_NotFound()
        {
            var genreId = 0;
            var seriesId = 1;
            _repository.Setup(t => t.DeleteSeriesAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

            var result = await _testedController.DeleteSeries(genreId, seriesId);

            _repository.Verify(m => m.DeleteSeriesAsync(genreId, seriesId), Times.Once());
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }
    }
}
