using AutoMapper;
using TVShowApplication.Data.DTO;
using TVShowApplication.Data.DTO.Series;
using TVShowApplication.Models;

namespace TVShowApplication.Data.Mapping
{
    public class SeriesProfile : Profile
    {
        public SeriesProfile()
        {
            CreateMap<Series, GetSeriesDto>()
                .ForMember(dest => dest.Poster, opt => opt.MapFrom(src => new Link { Href = $"/poster/{src.Poster.Id}" }))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres.Select(x => $"/genre/{x.Id}")))
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews.Select(x => $"/review/{x.Id}")));
            CreateMap<CreateSeriesDto, Series>()
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres.Select(x => new Genre { Id = x })))
                .ForMember(dest => dest.Poster, opt => opt.MapFrom(src => new Poster { Id = src.Poster }));
            CreateMap<UpdateSeriesDto, Series>();
        }
    }
}
