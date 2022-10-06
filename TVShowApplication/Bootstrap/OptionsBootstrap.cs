using TVShowApplication.Data.Options;

namespace TVShowApplication.Bootstrap
{
    public static class OptionsBootstrap
    {
        public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.Jwt));
        }
    }
}
