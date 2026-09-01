using ExteriorServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var hasExplicitUrls = !string.IsNullOrWhiteSpace(builder.Configuration["urls"]);
if (!hasExplicitUrls)
{
    builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");
}

builder.Services.AddControllers();

builder.Services.AddDbContext<ExteriorServicesDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Urls.Any(url => url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();