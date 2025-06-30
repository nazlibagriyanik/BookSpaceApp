using bookspace.Data;
using bookspace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bookspace.Areas.Member.Controllers
{
    [Area("Member")]
    [Authorize]
    public class MemberController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public MemberController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> MyDiscussions()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var discussions = await _context.Discussions
                .Include(d => d.Book)
                .Include(d => d.Comments)
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return View(discussions);
        }

        /// <summary>
        /// Kullanıcının favori kategorilerine göre kitap önerileri sunar.
        /// Basit içerik tabanlı öneri algoritması (ML bonusu için)
        /// </summary>
        public async Task<IActionResult> BookRecommendations()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            // Kullanıcının favori kitaplarının kategorilerini bul
            var favoriteCategoryIds = await _context.Books
                .Where(b => b.Discussions.Any(d => d.UserId == userId) ||
                            b.Discussions.Any(d => d.Comments.Any(c => c.UserId == userId)))
                .Select(b => b.CategoryId)
                .Distinct()
                .ToListAsync();

            // Bu kategorilerdeki popüler kitapları öner
            var recommendedBooks = await _context.Books
                .Where(b => favoriteCategoryIds.Contains(b.CategoryId))
                .OrderByDescending(b => b.Discussions.Count)
                .Take(5)
                .Include(b => b.Category)
                .ToListAsync();

            // Eğer hiç favori kategori yoksa, en popüler kitapları öner
            if (!recommendedBooks.Any())
            {
                recommendedBooks = await _context.Books
                    .OrderByDescending(b => b.Discussions.Count)
                    .Take(5)
                    .Include(b => b.Category)
                    .ToListAsync();
            }

            return View(recommendedBooks);
        }
    }
} 