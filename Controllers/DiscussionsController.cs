using bookspace.Data;
using bookspace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace bookspace.Controllers
{
    public class DiscussionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DiscussionsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Discussions
        public async Task<IActionResult> Index(int? bookId, string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentBook"] = bookId;

            var discussions = from d in _context.Discussions
                             .Include(d => d.Book)
                             .Include(d => d.User)
                             .Include(d => d.Comments)
                              select d;

            if (bookId.HasValue)
            {
                discussions = discussions.Where(d => d.BookId == bookId);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                discussions = discussions.Where(s => s.Title.Contains(searchString) || s.Content.Contains(searchString));
            }

            discussions = discussions.OrderByDescending(d => d.CreatedAt);

            ViewBag.Books = new SelectList(_context.Books, "Id", "Title");
            return View(await discussions.AsNoTracking().ToListAsync());
        }

        // GET: Discussions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussion = await _context.Discussions
                .Include(d => d.Book)
                .Include(d => d.User)
                .Include(d => d.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (discussion == null)
            {
                return NotFound();
            }

            return View(discussion);
        }

        // GET: Discussions/Create
        [Authorize]
        public IActionResult Create(int? bookId)
        {
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", bookId);
            return View();
        }

        // POST: Discussions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Title,Content,BookId")] Discussion discussion)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            if (ModelState.IsValid)
            {
                discussion.UserId = userId;
                discussion.CreatedAt = DateTime.Now;
                _context.Add(discussion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = discussion.Id });
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", discussion.BookId);
            return View(discussion);
        }

        // GET: Discussions/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussion = await _context.Discussions.FindAsync(id);
            if (discussion == null)
            {
                return NotFound();
            }

            // Check if user is the author or admin
            var currentUserId = _userManager.GetUserId(User);
            if (discussion.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", discussion.BookId);
            return View(discussion);
        }

        // POST: Discussions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,BookId,UserId,CreatedAt")] Discussion discussion)
        {
            if (id != discussion.Id)
            {
                return NotFound();
            }

            // Check if user is the author or admin
            var currentUserId = _userManager.GetUserId(User);
            if (discussion.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(discussion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscussionExists(discussion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = discussion.Id });
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", discussion.BookId);
            return View(discussion);
        }

        // GET: Discussions/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussion = await _context.Discussions
                .Include(d => d.Book)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discussion == null)
            {
                return NotFound();
            }

            // Check if user is the author or admin
            var currentUserId = _userManager.GetUserId(User);
            if (discussion.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(discussion);
        }

        // POST: Discussions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discussion = await _context.Discussions.FindAsync(id);
            if (discussion != null)
            {
                // Check if user is the author or admin
                var currentUserId = _userManager.GetUserId(User);
                if (discussion.UserId != currentUserId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                _context.Discussions.Remove(discussion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Discussions/AddComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddComment(int discussionId, string content)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            if (string.IsNullOrEmpty(content))
            {
                return RedirectToAction(nameof(Details), new { id = discussionId });
            }

            var comment = new Comment
            {
                DiscussionId = discussionId,
                UserId = userId,
                Content = content,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = discussionId });
        }

        // POST: Discussions/DeleteComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int commentId, int discussionId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment != null)
            {
                // Check if user is the author or admin
                var currentUserId = _userManager.GetUserId(User);
                if (currentUserId == null) return Challenge();

                if (comment.UserId != currentUserId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = discussionId });
        }

        private bool DiscussionExists(int id)
        {
            return _context.Discussions.Any(e => e.Id == id);
        }
    }
}
