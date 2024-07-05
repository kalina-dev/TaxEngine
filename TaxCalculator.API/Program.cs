using SQLitePCL;
using Serilog;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using TaxCalculator.API.Configurations;
using TaxCalculator.Model.Interfaces.Caching;
using TaxCalculator.Model.Repositories;
using TaxCalculator.Model.Interfaces;
using TaxCalculator.Caching;
using TaxCalculator.Model.Entities;
using TaxCalculator.Infrastructure.Services;
using TaxCalculator.Infrastructure.Middleware;
using TaxCalculator.Infrastructure.Helpers;
using TaxCalculator.Model.Interfaces.Application;

Batteries.Init(); // Initialize SQLite

Log.Logger = new LoggerConfiguration() // initialize Serilog
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<Tax>(builder.Configuration.GetSection("TaxConfig"));

builder.Services.AddTransient<ITaxCalculationService, TaxCalculationService>();

builder.Services.AddScoped<ITaxCalculator, IncomeTaxService>();
builder.Services.AddScoped<ITaxCalculator, SocialTaxService>();
builder.Services.AddScoped<IHelperTaxService, HelperTaxService>();
builder.Services.AddTransient<ITaxRepository, TaxRepository>();

builder.Services.AddMemoryCache();
builder.Services.AddTransient<ICachingService, InMemoryCachingService>();

builder.Host.UseSerilog();

builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
builder.Services.AddControllersWithViews().
    AddJsonOptions(options =>
    {
       options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
       options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
var app = builder.Build();
app.UseMiddleware<ExceptionHandler>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", "Version: " + description.GroupName.ToUpperInvariant());
        }
    });  
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
