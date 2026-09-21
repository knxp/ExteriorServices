using ExteriorServices.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:5001";

builder.Services.AddRazorPages();
builder.Services.AddHttpClient<IDashboardApiClient, DashboardApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

builder.Services.AddHttpClient<ICustomerApiClient, CustomerApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

builder.Services.AddHttpClient<IVisualizationImageApiClient, VisualizationImageApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
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
