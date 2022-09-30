using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using TVShowApplication.Data.DTO;
using TVShowApplication.Services.Interfaces;
using TVShowApplication.Models;

namespace TVShowApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Models.User.Role)]
    public class GenreController : ControllerBase
    {
        private readonly IGenreRepository _repository;
        private readonly IMapper _mapper;

        public GenreController(IGenreRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetGenres()
        {
            var genres = await _repository.GetGenresAsync();

            return Ok(genres);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetGenreById(int id)
        {
            var genre = await _repository.GetGenreAsync(id);

            if (genre == null) return NotFound();

            return Ok(genre);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGenre(CreateGenreDto createGenreRequest)
        {
            var genre = _mapper.Map<Genre>(createGenreRequest);

            var createdGenre = await _repository.InsertGenreAsync(genre);

            if (createdGenre == null) return BadRequest();

            return CreatedAtAction(nameof(GetGenreById), _mapper.Map<>(createdGenre));
        }
    }
}
