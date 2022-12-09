using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using TVShowApplication.Bootstrap;
using TVShowApplication.Data;
using TVShowApplication.Exceptions;
using TVShowApplication.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails(options =>
{
    options.IncludeExceptionDetails = (ctx, ex) => builder.Environment.IsDevelopment();

    options.Map<UnauthorizedException>(ex => new ProblemDetails
    {
        Title = "Unauthorized",
        Status = StatusCodes.Status403Forbidden,
        Detail = ex.Message,
    });

    options.Map<ResourceNotFoundException>(ex => new ProblemDetails
    {
        Title = "NotFound",
        Status = StatusCodes.Status404NotFound,
        Detail = ex.Message,
    });

    options.Map<UnupdateableResourceException>(ex => new ProblemDetails
    {
        Title = "Cannot update resource",
        Status = StatusCodes.Status403Forbidden,
        Detail = ex.Message,
    });
});

// Add services to the container.
builder.Services.AddDbContext<TVShowContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddRepositories(builder.Configuration);
builder.Services.ConfigureOptions(builder.Configuration);

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "TV Show Application", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter a valid bearer token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer",
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

app.UseProblemDetails();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseWhen(c => !c.Request.Path.StartsWithSegments(new PathString("/api")), appBuilder =>
{
    appBuilder.UseSpa(spa =>
    {
        spa.Options.SourcePath = "ClientApp\\build";

        if (app.Environment.IsDevelopment())
        {
            spa.UseReactDevelopmentServer("start");
        }
    });
});

//app.MapWhen(req => !req.Request.Path.Value.StartsWith("/api"), appBuilder =>
//{
//    appBuilder.UseSpa(spa =>
//    {
//        spa.Options.SourcePath = Path.Combine(builder.Environment.ContentRootPath, "ClientApp/build");

//        if (builder.Environment.IsDevelopment())
//        {
//            spa.UseReactDevelopmentServer("start");
//        }
//    });
//});

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UserDataMiddleware>();

//app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

    endpoints.MapFallbackToFile("/index.html");
});

//app.UseSpa(configuration =>
//{
//    if (builder.Environment.IsDevelopment())
//    {
//        configuration.UseReactDevelopmentServer("start");
//    }

//    configuration.Options.DefaultPage = PathString.FromUriComponent("/");
//    configuration.Options.SourcePath = "ClientApp/build";
//});

app.Run();
