using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.Collections.Generic;
using TVShowApplication.Controllers;
using TVShowApplication.Data.DTO.Genre;
using TVShowApplication.Data.Options;
using TVShowApplication.Models;
using TVShowApplication.Services.Authentication;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Tests.UnitTests
{
    internal class GenreControllerTests
    {
        private Mock<IGenreRepository> _repository;
        private Mock<IMapper> _mapper;
        private GenreController _testedController;

        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IGenreRepository>();
            _mapper = new Mock<IMapper>();
            _testedController = new GenreController(_repository.Object, _mapper.Object);
        }

        [Test] 
        public async Task GetGenres_ShouldHandleSuccesfulCalls()
        {
            var genreDtos = new List<GetGenreDto>();
            var genres = new List<Genre>();
            genres.Add(new Genre());
            genreDtos.Add(new GetGenreDto());
            _repository.Setup(t => t.GetGenresAsync()).ReturnsAsync(genres);
            _mapper.Setup(t => t.Map<IEnumerable<GetGenreDto>>(It.IsAny<IEnumerable<Genre>>())).Returns(genreDtos);

            var result = await _testedController.GetGenres();

            _repository.Verify(m => m.GetGenresAsync(), Times.Once());
            _mapper.Verify(m => m.Map<IEnumerable<GetGenreDto>>(genres), Times.Once());
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            Assert.That((result as OkObjectResult)?.Value, Is.EqualTo(genreDtos));
        }

        [Test]
        public async Task GetGenreById_ShouldHandleSuccesfulCalls()
        {
            var id = 0;
            var genre = new Genre();
            var genreDto = new GetGenreDto();
            _repository.Setup(t => t.GetGenreAsync(It.IsAny<int>())).ReturnsAsync(genre);
            _mapper.Setup(t => t.Map<GetGenreDto>(It.IsAny<Genre>())).Returns(genreDto);

            var result = await _testedController.GetGenreById(id);

            _repository.Verify(m => m.GetGenreAsync(id), Times.Once());
            _mapper.Verify(m => m.Map<GetGenreDto>(genre), Times.Once());
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            Assert.That((result as OkObjectResult)?.Value, Is.EqualTo(genreDto));
        }

        [Test]
        public async Task GetGenreById_ShouldHandleNotFound()
        {
            var id = 0;
            var genreDto = new GetGenreDto();
            _repository.Setup(t => t.GetGenreAsync(It.IsAny<int>())).ReturnsAsync((Genre?)null);

            var result = await _testedController.GetGenreById(id);

            _repository.Verify(m => m.GetGenreAsync(id), Times.Once());
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task CreateGenre_Succesful ()
        {
            var genre = new Genre();
            var genreDto = new CreateGenreDto();
            _repository.Setup(t => t.InsertGenreAsync(It.IsAny<Genre>())).ReturnsAsync(genre);
            _mapper.Setup(t => t.Map<Genre>(It.IsAny<CreateGenreDto>())).Returns(genre);

            var result = await _testedController.CreateGenre(genreDto);

            _repository.Verify(m => m.InsertGenreAsync(genre), Times.Once());
            _mapper.Verify(m => m.Map<Genre>(genreDto), Times.Once());
            Assert.That(result, Is.TypeOf<CreatedAtActionResult>());
        }

        [Test]
        public async Task CreateGenre_Failing()
        {
            var genre = new Genre();
            var genreDto = new CreateGenreDto();
            _repository.Setup(t => t.InsertGenreAsync(It.IsAny<Genre>())).ReturnsAsync((Genre?)null);
            _mapper.Setup(t => t.Map<Genre>(It.IsAny<CreateGenreDto>())).Returns(genre);

            var result = await _testedController.CreateGenre(genreDto);

            _repository.Verify(m => m.InsertGenreAsync(genre), Times.Once());
            _mapper.Verify(m => m.Map<Genre>(genreDto), Times.Once());
            Assert.That(result, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        public async Task UpdateGenre_Succesful()
        {
            var id = 0;
            var genre = new Genre();
            var genreDto = new UpdateGenreDto();
            _repository.Setup(t => t.UpdateGenreAsync(It.IsAny<int>(), It.IsAny<Genre>())).ReturnsAsync(true);
            _mapper.Setup(t => t.Map<Genre>(It.IsAny<UpdateGenreDto>())).Returns(genre);

            var result = await _testedController.UpdateGenre(id, genreDto);

            _repository.Verify(m => m.UpdateGenreAsync(id, genre), Times.Once());
            _mapper.Verify(m => m.Map<Genre>(genreDto), Times.Once());
            Assert.That(result, Is.TypeOf<OkResult>());
        }


        [Test]
        public async Task UpdateGenre_NotFound()
        {
            var id = 0;
            var genre = new Genre();
            var genreDto = new UpdateGenreDto();
            _repository.Setup(t => t.UpdateGenreAsync(It.IsAny<int>(), It.IsAny<Genre>())).ReturnsAsync(false);
            _mapper.Setup(t => t.Map<Genre>(It.IsAny<UpdateGenreDto>())).Returns(genre);

            var result = await _testedController.UpdateGenre(id, genreDto);

            _repository.Verify(m => m.UpdateGenreAsync(id, genre), Times.Once());
            _mapper.Verify(m => m.Map<Genre>(genreDto), Times.Once());
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteGenre_Succesful()
        {
            var id = 0;
            _repository.Setup(t => t.DeleteGenreAsync(It.IsAny<int>())).ReturnsAsync(true);

            var result = await _testedController.DeleteGenre(id);

            _repository.Verify(m => m.DeleteGenreAsync(id), Times.Once());
            Assert.That(result, Is.TypeOf<OkResult>());
        }

        [Test]
        public async Task DeleteGenre_NotFound()
        {
            var id = 0;
            _repository.Setup(t => t.DeleteGenreAsync(It.IsAny<int>())).ReturnsAsync(false);

            var result = await _testedController.DeleteGenre(id);

            _repository.Verify(m => m.DeleteGenreAsync(id), Times.Once());
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }
    }
}
