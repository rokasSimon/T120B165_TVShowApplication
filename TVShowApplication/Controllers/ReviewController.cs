using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TVShowApplication.Attributes;
using TVShowApplication.Data.DTO.Review;
using TVShowApplication.Models;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _repository;
        private readonly IMapper _mapper;

        public ReviewController(IReviewRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        [AuthorizeRoles(Role.User, Role.Poster, Role.Admin)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetReviews()
        {
            var review = await _repository.GetReviewAsync();

            return Ok(_mapper.Map<IEnumerable<GetReviewDto>>(review));
        }

        [HttpGet("{id:int}")]
        [AuthorizeRoles(Role.User, Role.Poster, Role.Admin)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetReviewById(int id)
        {
            var review = await _repository.GetReviewAsync(id);

            if (review == null) return NotFound();

            return Ok(_mapper.Map<GetReviewDto>(review));
        }

        [HttpPost]
        [AuthorizeRoles(Role.Admin, Role.Poster, Role.User)]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto createReviewRequest)
        {
            var review = _mapper.Map<Review>(createReviewRequest);

            var createdReview = await _repository.InsertReviewAsync(review);
            if (createdReview == null) return BadRequest();

            return CreatedAtAction(nameof(GetReviewById), new { id = createdReview.Id }, _mapper.Map<GetReviewDto>(createdReview));
        }

        [HttpPut("{id:int}")]
        [AuthorizeRoles(Role.Admin, Role.Poster, Role.User)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] UpdateReviewDto updateReviewRequest)
        {
            var review = _mapper.Map<Review>(updateReviewRequest);

            var success = await _repository.UpdateReviewAsync(id, review);

            if (!success) return NotFound();

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [AuthorizeRoles(Role.Admin, Role.Poster, Role.User)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var success = await _repository.DeleteReviewAsync(id);

            if (!success) return NotFound();

            return Ok();
        }
    }
}
