using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExteriorServices.Web.Pages;

public sealed class ImagesModel : PageModel
{
    private readonly IConfiguration _configuration;

    public ImagesModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string BrowserApiBaseUrl => _configuration["BrowserApiBaseUrl"]
        ?? _configuration["ApiBaseUrl"]
        ?? "https://localhost:5001";
}
