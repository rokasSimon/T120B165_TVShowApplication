using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Models.User.Role)]
    public class GenreController : ControllerBase
    {
        private readonly IGenreRepository _repository;

        public GenreController(IGenreRepository repository)
        {
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

        //[HttpPost]
        //public async Task<IActionResult> CreateGenre(object createGenreRequest)
        //{
        //    var genreId = await _repository.InsertGenreAsync(null);

        //    if (genreId == null) return BadRequest();

        //    return CreatedAtAction()
        //}
    }
}
