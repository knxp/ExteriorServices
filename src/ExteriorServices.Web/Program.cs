using ExteriorServices.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpClient<IDashboardApiClient, DashboardApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5000");
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.Run();
