using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using learnFromFiles.Models;
using learnFromFiles.Services;
namespace learnFromFiles.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    [BindProperty]
    public UserModel Usuario { get; set; } = new UserModel("", "");
    private readonly FileService _fileService;

    [BindProperty]
    public IFormFile? Upload { get; set; }
    [BindProperty]
   public string SearchKey { get; set; } = "";

    public IEnumerable<string> Files { get; private set; } = Enumerable.Empty<string>(); //filel ist 
   public IDictionary<string, string> SearchResults { get; private set; } = new Dictionary<string, string>();

    public IndexModel(ILogger<IndexModel> logger, FileService fileService)
    {
        _fileService = fileService;
        _logger = logger;
    }

    public void OnGet()
    {
        Files = _fileService.GetAllFileNames();
    }
    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();

        Usuario.Greeting = $"¡Hola, {Usuario.Name}! Bienvenido/a a Razor Pages.";
        return Page();  // re‑renderiza con la propiedad Saludo actualizada
    }
        public async Task<IActionResult> OnPostUploadAsync()
        {
        Console.WriteLine($"we are uploading file with {Upload.FileName}");
            if (Upload != null && Upload.Length > 0)
            await _fileService.SaveFileAsync(Upload);

            // Recargar lista de archivos
            Files = _fileService.GetAllFileNames();
            return RedirectToPage();
        }

    public IActionResult OnPostSearch()
    {
        Files = _fileService.GetAllFileNames();
        if (!string.IsNullOrWhiteSpace(SearchKey))
        {
            SearchResults = _fileService.SearchWithLine(SearchKey);
        }
        return Page();
    }
}
