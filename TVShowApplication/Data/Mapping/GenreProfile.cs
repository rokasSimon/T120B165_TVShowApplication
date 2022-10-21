using AutoMapper;
using TVShowApplication.Data.DTO.Genre;
using TVShowApplication.Models;

namespace TVShowApplication.Data.Mapping
{
    public class GenreProfile : Profile
    {
        public GenreProfile()
        {
            CreateMap<CreateGenreDto, Genre>();
            CreateMap<Genre, GetGenreDto>()
                .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.Videos.Select(x => $"/genre/{src.Id}/series/{x.Id}")));
            CreateMap<UpdateGenreDto, Genre>();
        }
    }
}
