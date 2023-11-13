using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using FlashcardsApp.Models;
using FlashcardsApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly SearchService _searchService;

        public HomeController(ILogger<HomeController> logger, SearchService searchService) 
        {
            _logger = logger;
            _searchService = searchService;
            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7296/api") };
        }

        [HttpGet]
        public IActionResult Search(string? search)
        {
            var allCollections = HttpApiService.GetFromAPI<List<FlashcardCollection<Flashcards>>?>(_httpClient, "/FlashcardCollections/GetFlashcardCollections");
            if (allCollections == null || !allCollections.Any())
            {
                return View("ErrorView");
            }

            TempData["LastSearchQuery"] = search;

            if (!string.IsNullOrEmpty(search))
            {
                var matchingCollections = _searchService.FilterBySearchTerm(allCollections, search);
                if (!matchingCollections.Any())
                {
                    TempData["EmptyResults"] = "No results found for the search query.";
                }
                return View("SearchView", matchingCollections);
            }

            return View("SearchView", allCollections);
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel(Activity.Current?.Id ?? HttpContext.TraceIdentifier));
        }
    }
}


   

    