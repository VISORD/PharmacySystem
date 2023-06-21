using System.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using PharmacySystem.WebAPI.Authentication;
using PharmacySystem.WebAPI.Authentication.Claims;
using PharmacySystem.WebAPI.Database;
using PharmacySystem.WebAPI.Database.Publisher;
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
        // Configure options
        builder.Services.ConfigureOptions<ApplicationOptionsSetup>();

        // Configure database context
        builder.Services
            .AddDbContext<DatabaseContext>()
            .AddScoped<IDatabasePublicationService, DatabasePublicationService>();

        // Add filters
        builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IStartupFilter, DatabasePublicationStartupFilter>());

        // Configure authentication and authorization
        builder.Services.AddTransient<AuthenticationEvents>();
        builder.Services
            .AddAuthentication(options => options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = ".PharmacySystem.Cookies";
                options.Cookie.MaxAge = TimeSpan.FromHours(1);
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.HttpOnly = true;
                options.SlidingExpiration = true;
                options.EventsType = typeof(AuthenticationEvents);
            });

        builder.Services.AddAuthorization();

        // Configure controllers and routing
        builder.Services.AddControllers(options =>
        {
            // Include context provider
            options.ModelBinderProviders.Insert(0, new ClaimProvider());
        });

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