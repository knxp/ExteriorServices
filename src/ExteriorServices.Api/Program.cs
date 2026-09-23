using ExteriorServices.Api.Errors;
using ExteriorServices.Api.Middleware;
using ExteriorServices.Api.Configuration;
using ExteriorServices.Application;
using ExteriorServices.Application.Configuration;
using ExteriorServices.Infrastructure;
using ExteriorServices.Infrastructure.Data;
using ExteriorServices.Api.Services;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// User secrets are only auto-loaded when ASPNETCORE_ENVIRONMENT=Development, but this app
// runs without launchSettings.json, so load them explicitly regardless of environment.
builder.Configuration.AddUserSecrets<Program>(optional: true);

AppConfigurationValidator.Validate(builder.Configuration);

var hasExplicitUrls = !string.IsNullOrWhiteSpace(builder.Configuration["urls"]);
if (!hasExplicitUrls)
{
    builder.WebHost.UseUrls("http://0.0.0.0:5000", "https://0.0.0.0:5001");
}

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = "ExteriorServices.Admin";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(options => options.AddPolicy("LocalWeb", policy =>
{
    policy.WithOrigins(
            "http://localhost:5010",
            "https://localhost:5010",
            "http://192.168.1.68:5010",
            "https://exteriorservices-web-hyemffhacnendzgs.centralus-01.azurewebsites.net")
        .AllowAnyHeader()
        .AllowAnyMethod();
}));
builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<AzureStorageOptions>(builder.Configuration.GetSection(AzureStorageOptions.SectionName));
var azureStorageConnectionString = builder.Configuration["AzureStorage:ConnectionString"];
if (!string.IsNullOrWhiteSpace(azureStorageConnectionString))
{
    var containerName = builder.Configuration["AzureStorage:ContainerName"];
    containerName = string.IsNullOrWhiteSpace(containerName) ? "visualizations" : containerName;
    builder.Services.AddSingleton(_ =>
    {
        var containerClient = new BlobContainerClient(azureStorageConnectionString, containerName);
        containerClient.CreateIfNotExists(PublicAccessType.None);
        return containerClient;
    });
    builder.Services.AddScoped<IPropertyVisualizationIntakeService, BlobPropertyVisualizationIntakeService>();
    builder.Services.AddSingleton<IStoredVisualizationImageService, BlobStoredVisualizationImageService>();
}
else
{
    builder.Services.AddScoped<IPropertyVisualizationIntakeService, LocalPropertyVisualizationIntakeService>();
    builder.Services.AddSingleton<IStoredVisualizationImageService, StoredVisualizationImageService>();
}

builder.Services.Configure<OpenAIOptions>(builder.Configuration.GetSection(OpenAIOptions.SectionName));
builder.Services.AddHttpClient(OpenAIPropertyVisualizationRenderingService.HttpClientName, client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/v1/");
    client.Timeout = TimeSpan.FromMinutes(3);
});
builder.Services.AddScoped<IPropertyVisualizationRenderingService, OpenAIPropertyVisualizationRenderingService>();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value is not null && entry.Value.Errors.Count > 0)
            .SelectMany(entry => entry.Value!.Errors.Select(error => new
            {
                Field = entry.Key,
                Error = error.ErrorMessage ?? "Validation failed."
            }))
            .ToList();

        return new BadRequestObjectResult(new
        {
            error = "ValidationError",
            message = "One or more validation errors occurred.",
            errors
        });
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ExteriorServicesDbContext>();

    if (dbContext.Database.IsSqlite())
    {
        dbContext.Database.EnsureCreated();
    }
    else
    {
        dbContext.Database.Migrate();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseApiExceptionHandling();
app.UseSwagger();
app.UseSwaggerUI();
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors("LocalWeb");
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();