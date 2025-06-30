using bookspace.Data;
using bookspace.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bookspace.ViewComponents
{
    public class RecentBooksViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public RecentBooksViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int count = 5)
        {
            var recentBooks = await _context.Books
                .Include(b => b.Category)
                .OrderByDescending(b => b.Id)
                .Take(count)
                .ToListAsync();

            return View(recentBooks);
        }
    }
} 