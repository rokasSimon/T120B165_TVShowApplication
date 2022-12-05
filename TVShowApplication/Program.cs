using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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
});

builder.Services.AddSpaStaticFiles(spa =>
{
    spa.RootPath = "ClientApp";
});

var app = builder.Build();

app.UseProblemDetails();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSpaStaticFiles();

app.MapWhen(req => !req.Request.Path.Value.StartsWith("/api"), appBuilder =>
{
    appBuilder.UseSpa(spa =>
    {
        spa.Options.SourcePath = Path.Combine(builder.Environment.ContentRootPath, "ClientApp");

        if (builder.Environment.IsDevelopment())
        {
            spa.UseReactDevelopmentServer("start");
        }
    });
});

app.Use(async (req, next) =>
{
    var headers = req.Request.Headers.ToList();

    foreach (var item in headers)
    {
        Console.WriteLine($"{item.Key}: {item.Value}");
    }

    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UserDataMiddleware>();

app.MapControllers();

app.Run();
