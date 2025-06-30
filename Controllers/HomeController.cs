using bookspace.Data;
using bookspace.Models;
using bookspace.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace bookspace.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel();
            try
            {
                viewModel.RecentBooks = await _context.Books
                    .Include(b => b.Category)
                    .OrderByDescending(b => b.Id)
                    .Take(6)
                    .ToListAsync();

                viewModel.PopularCategories = await _context.Categories
                    .Include(c => c.Books)
                    .OrderByDescending(c => c.Books.Count)
                    .Take(4)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Home Index action");
                // The view model is already initialized with empty lists.
            }
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
