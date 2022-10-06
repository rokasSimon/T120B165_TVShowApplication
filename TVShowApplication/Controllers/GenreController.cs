using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TVShowApplication.Services.Interfaces;
using TVShowApplication.Models;
using TVShowApplication.Data.DTO.Genre;
using TVShowApplication.Attributes;

namespace TVShowApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [AuthorizeRoles(Role.User, Role.Poster, Role.Admin)]
        public async Task<IActionResult> GetGenres()
        {
            var genres = await _repository.GetGenresAsync();

            return Ok(_mapper.Map<IEnumerable<GetGenreDto>>(genres));
        }

        [HttpGet("{id:int}")]
        [AuthorizeRoles(Role.User, Role.Poster, Role.Admin)]
        public async Task<IActionResult> GetGenreById(int id)
        {
            var genre = await _repository.GetGenreAsync(id);

            if (genre == null) return NotFound();

            return Ok(_mapper.Map<GetGenreDto>(genre));
        }

        [HttpPost]
        [AuthorizeRoles(Role.Admin)]
        public async Task<IActionResult> CreateGenre([FromBody] CreateGenreDto createGenreRequest)
        {
            var genre = _mapper.Map<Genre>(createGenreRequest);

            var createdGenre = await _repository.InsertGenreAsync(genre);
            if (createdGenre == null) return BadRequest();

            return CreatedAtAction(nameof(GetGenreById), new { id = createdGenre.Id }, _mapper.Map<GetGenreDto>(createdGenre));
        }

        [HttpPut("{id:int}")]
        [AuthorizeRoles(Role.Admin)]
        public async Task<IActionResult> UpdateGenre(int id, [FromBody] UpdateGenreDto updateGenreRequest)
        {
            var genre = _mapper.Map<Genre>(updateGenreRequest);

            var success = await _repository.UpdateGenreAsync(id, genre);

            if (!success) return NotFound();

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [AuthorizeRoles(Role.Admin)]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var success = await _repository.DeleteGenreAsync(id);

            if (!success) return NotFound();

            return Ok();
        }
    }
}
