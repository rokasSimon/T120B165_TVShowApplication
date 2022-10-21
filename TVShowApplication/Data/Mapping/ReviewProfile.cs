using AutoMapper;
using TVShowApplication.Data.DTO;
using TVShowApplication.Models;
using TVShowApplication.Data.DTO.Review;

namespace TVShowApplication.Data.Mapping
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<Review, GetReviewDto>()
                .ForMember(dest => dest.ReviewedSeries, opt => opt.MapFrom(src => new Link { Href = $"/genre/{src.ReviewedSeries.Genres.First().Id}/series/{src.ReviewedSeries.Id}" }))
                .ForMember(dest => dest.Reviewer, opt => opt.MapFrom(src => src.Reviewer == null ? null : new Link { Href = $"/user/{src.Reviewer.Id}" }));
            CreateMap<CreateReviewDto, Review>()
                .ForMember(dest => dest.ReviewedSeries, opt => opt.MapFrom(src => new Series { Id = src.Series }))
                .ForMember(dest => dest.Reviewer, opt => opt.MapFrom(src => new User { Id = src.User }));
            CreateMap<UpdateReviewDto, Review>();
        }
    }
}
