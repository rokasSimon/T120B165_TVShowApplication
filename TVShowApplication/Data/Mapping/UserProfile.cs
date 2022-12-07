using AutoMapper;
using TVShowApplication.Data.DTO.User;
using TVShowApplication.Models;

namespace TVShowApplication.Data.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, GetUserDTO>()
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews.Select(x => $"/genre/{x.ReviewedSeries.Genres.First().Id}/series/{x.ReviewedSeries.Id}/review/{x.Id}")));
        }
    }
}
