using ExteriorServices.Application;
using ExteriorServices.Api.Errors;
using ExteriorServices.Api.Middleware;
using ExteriorServices.Infrastructure;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var hasExplicitUrls = !string.IsNullOrWhiteSpace(builder.Configuration["urls"]);
if (!hasExplicitUrls)
{
    builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");
}

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
        new BadRequestObjectResult(new ApiError(
            "ValidationError",
            "One or more validation errors occurred."));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseApiExceptionHandling();
app.UseSwagger();
app.UseSwaggerUI();

if (app.Urls.Any(url => url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();