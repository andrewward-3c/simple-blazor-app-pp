using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PokerPlanning.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public bool IsDevelopment { get; set; }

    private readonly ILogger<ErrorModel> _logger;
    private readonly IWebHostEnvironment _environment;

    public ErrorModel(ILogger<ErrorModel> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        IsDevelopment = _environment.IsDevelopment();
    }
}
