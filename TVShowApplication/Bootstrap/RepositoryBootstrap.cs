using TVShowApplication.Data;
using TVShowApplication.Services.Database;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Bootstrap
{
    public static class RepositoryBootstrap
    {
        public static void AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGenreRepository, GenreRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<ISeriesRepository, SeriesRepository>();

            services.AddDbContext<TVShowContext>();
        }
    }
}
