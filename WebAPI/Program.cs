using System.Reflection;
using Microsoft.OpenApi.Models;
using PharmacySystem.WebAPI.Options;

namespace PharmacySystem.WebAPI;

internal static class Program
{
    private const string ApiVersion = "latest";

    public static void Main(string[] args) => WebApplication.CreateBuilder(args)
        .ConfigureServices()
        .ConfigureApplication()
        .Run();

    /// <summary>
    /// Add services to the container.
    /// </summary>
    /// <param name="builder">Web Application builder.</param>
    private static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        // Configure authorization
        builder.Services.AddAuthorization();

        // Configure routing
        builder.Services.AddControllers();
        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        // See more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            // Service description
            options.SwaggerDoc(ApiVersion, new OpenApiInfo
            {
                Title = "Pharmacy System API",
                Version = "1.0",
            });

            // Load XML documentation for Swagger
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
        });

        return builder.Build();
    }

    /// <summary>
    /// Configure the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Built Web Application instance.</param>
    /// <returns>Configured Web Application instance.</returns>
    private static WebApplication ConfigureApplication(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                // API version configuration
                options.SwaggerEndpoint($"/swagger/{ApiVersion}/swagger.json", ApiVersion);
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}