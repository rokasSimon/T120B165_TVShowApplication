using AutoMapper;
using TVShowApplication.Data.DTO;
using TVShowApplication.Models;

namespace TVShowApplication.Data.Mapping
{
    public class GenreProfile : Profile
    {
        public GenreProfile()
        {
            CreateMap<CreateGenreDto, Genre>();
            CreateMap<Genre, GetGenreDto>();
        }
    }
}
