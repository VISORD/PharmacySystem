using System.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using PharmacySystem.WebAPI.Authentication;
using PharmacySystem.WebAPI.Authentication.Claims;
using PharmacySystem.WebAPI.Database.Connection;
using PharmacySystem.WebAPI.Database.Publisher;
using PharmacySystem.WebAPI.Database.Repositories;
using PharmacySystem.WebAPI.Models.Common;
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
            .AddScoped<IAccountRepository, AccountRepository>()
            .AddScoped<ICompanyRepository, CompanyRepository>()
            .AddScoped<IMedicamentRepository, MedicamentRepository>()
            .AddScoped<IMedicamentAnalogueRepository, MedicamentAnalogueRepository>()
            .AddScoped<IOrderRepository, OrderRepository>()
            .AddScoped<IOrderHistoryRepository, OrderHistoryRepository>()
            .AddScoped<IOrderMedicamentRepository, OrderMedicamentRepository>()
            .AddScoped<IPharmacyRepository, PharmacyRepository>()
            .AddScoped<IPharmacyWorkingHoursRepository, PharmacyWorkingHoursRepository>()
            .AddScoped<IPharmacyMedicamentRepository, PharmacyMedicamentRepository>()
            .AddScoped<IPharmacyMedicamentRateRepository, PharmacyMedicamentRateRepository>()
            .AddScoped<IPharmacyMedicamentSaleRepository, PharmacyMedicamentSaleRepository>()
            .AddScoped<IPharmacyMedicamentOrderRepository, PharmacyMedicamentOrderRepository>()
            .AddScoped<IDatabasePublicationService, DatabasePublicationService>();

        // Add filters
        builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IStartupFilter, DatabasePublicationStartupFilter>());

        // Configure CORS
        builder.Services.AddCors(options => options.AddDefaultPolicy(cors =>
        {
            cors.WithOrigins("http://localhost:5173", "https://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }));

        // Configure authentication and authorization
        builder.Services.AddTransient<AuthenticationEvents>();
        builder.Services
            .AddAuthentication(options => options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = ".PharmacySystem.Cookies";
                options.Cookie.MaxAge = TimeSpan.FromHours(1);
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.HttpOnly = true;
                options.SlidingExpiration = true;
                options.EventsType = typeof(AuthenticationEvents);
            });

        builder.Services.AddAuthorization();

        // Configure controllers and routing
        builder.Services.AddControllers(options =>
        {
            // Include providers
            options.ModelBinderProviders.Insert(0, new ClaimProvider());
            options.ModelBinderProviders.Insert(1, new DatabaseConnectionProvider());
        }).ConfigureApiBehaviorOptions(options => options.InvalidModelStateResponseFactory = context =>
        {
            var response = new ItemResponse(new SerializableError(context.ModelState), "One or more model validation errors occured");
            return new BadRequestObjectResult(response);
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

        app.UseCors();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}