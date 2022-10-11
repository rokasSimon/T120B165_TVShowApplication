using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TVShowApplication.Attributes;
using TVShowApplication.Data.DTO.Series;
using TVShowApplication.Models;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SeriesController : ControllerBase
    {
        private readonly ISeriesRepository _repository;
        private readonly IMapper _mapper;

        // TODO: {{hostname}}/api/genre/2/series/1/reviews

        public SeriesController(ISeriesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        [AuthorizeRoles(Role.User, Role.Poster, Role.Admin)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetSeries()
        {
            var series = await _repository.GetSeriesAsync();

            return Ok(_mapper.Map<IEnumerable<GetSeriesDto>>(series));
        }

        [HttpGet("{id:int}")]
        [AuthorizeRoles(Role.User, Role.Poster, Role.Admin)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetSeriesById(int id)
        {
            var series = await _repository.GetSeriesAsync(id);

            if (series == null) return NotFound();

            return Ok(_mapper.Map<GetSeriesDto>(series));
        }

        [HttpPost]
        [AuthorizeRoles(Role.Admin, Role.Poster)]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateSeries([FromBody] CreateSeriesDto createSeriesRequest)
        {
            var series = _mapper.Map<Series>(createSeriesRequest);

            var createdSeries = await _repository.InsertSeriesAsync(series);
            if (createdSeries == null) return BadRequest();

            return CreatedAtAction(nameof(GetSeriesById), new { id = createdSeries.Id }, _mapper.Map<GetSeriesDto>(createdSeries));
        }

        [HttpPatch("{id:int}")]
        [AuthorizeRoles(Role.Admin, Role.Poster)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateSeries(int id, [FromBody] UpdateSeriesDto updateSeriesRequest)
        {
            var series = _mapper.Map<Series>(updateSeriesRequest);

            var success = await _repository.UpdateSeriesAsync(id, series);

            if (!success) return NotFound();

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [AuthorizeRoles(Role.Admin, Role.Poster)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteSeries(int id)
        {
            var success = await _repository.DeleteSeriesAsync(id);

            if (!success) return NotFound();

            return Ok();
        }
    }
}
