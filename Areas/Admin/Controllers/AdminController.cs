using bookspace.Data;
using bookspace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bookspace.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardViewModel = new AdminDashboardViewModel
            {
                TotalUsers = await _userManager.Users.CountAsync(),
                TotalBooks = await _context.Books.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync(),
                TotalDiscussions = await _context.Discussions.CountAsync(),
                TotalComments = await _context.Comments.CountAsync(),
                RecentUsers = await _userManager.Users.OrderByDescending(u => u.CreatedAt).Take(5).ToListAsync(),
                RecentBooks = await _context.Books.Include(b => b.Category).OrderByDescending(b => b.Id).Take(5).ToListAsync()
            };

            return View(dashboardViewModel);
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    Roles = roles.ToList()
                });
            }

            return View(userViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUserRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var isInRole = await _userManager.IsInRoleAsync(user, roleName);
            if (isInRole)
            {
                await _userManager.RemoveFromRoleAsync(user, roleName);
            }
            else
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }

            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> Statistics()
        {
            var statistics = new StatisticsViewModel
            {
                BooksByCategory = await _context.Books
                    .GroupBy(b => b.Category.Name)
                    .Select(g => new CategoryBookCount { CategoryName = g.Key, BookCount = g.Count() })
                    .ToListAsync(),
                DiscussionsByMonth = await _context.Discussions
                    .GroupBy(d => new { d.CreatedAt.Year, d.CreatedAt.Month })
                    .Select(g => new MonthlyDiscussionCount 
                    { 
                        Month = $"{g.Key.Year}-{g.Key.Month:D2}", 
                        DiscussionCount = g.Count() 
                    })
                    .OrderByDescending(x => x.Month)
                    .Take(12)
                    .ToListAsync()
            };

            return View(statistics);
        }
    }

    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalBooks { get; set; }
        public int TotalCategories { get; set; }
        public int TotalDiscussions { get; set; }
        public int TotalComments { get; set; }
        public List<User> RecentUsers { get; set; } = new();
        public List<Book> RecentBooks { get; set; } = new();
    }

    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }

    public class StatisticsViewModel
    {
        public List<CategoryBookCount> BooksByCategory { get; set; } = new();
        public List<MonthlyDiscussionCount> DiscussionsByMonth { get; set; } = new();
    }

    public class CategoryBookCount
    {
        public string CategoryName { get; set; } = string.Empty;
        public int BookCount { get; set; }
    }

    public class MonthlyDiscussionCount
    {
        public string Month { get; set; } = string.Empty;
        public int DiscussionCount { get; set; }
    }
} 