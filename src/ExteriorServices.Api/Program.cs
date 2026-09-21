using ExteriorServices.Api.Errors;
using ExteriorServices.Api.Middleware;
using ExteriorServices.Application;
using ExteriorServices.Application.Configuration;
using ExteriorServices.Infrastructure;
using ExteriorServices.Infrastructure.Data;
using ExteriorServices.Api.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

AppConfigurationValidator.Validate(builder.Configuration);

var hasExplicitUrls = !string.IsNullOrWhiteSpace(builder.Configuration["urls"]);
if (!hasExplicitUrls)
{
    builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");
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
            "http://192.168.1.68:5010")
        .AllowAnyHeader()
        .AllowAnyMethod();
}));
builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IPropertyVisualizationIntakeService, LocalPropertyVisualizationIntakeService>();
builder.Services.AddSingleton<IStoredVisualizationImageService, StoredVisualizationImageService>();
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