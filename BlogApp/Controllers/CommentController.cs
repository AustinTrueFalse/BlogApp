using BlogApp.Models.Services;
using BlogApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogApp.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly ApplicationDbContext _context;

        public CommentController(ICommentService commentService, ApplicationDbContext context)
        {
            _commentService = commentService;
            _context = context;
        }

        // GET: Comment
        public async Task<IEnumerable<Comment>> GetAllCommentsAsync()
        {
            var comments = await _context.Comments.ToListAsync();
            if (comments == null)
            {
                Console.WriteLine("Комментарии не найдены в сервисе");
                comments = new List<Comment>(); // Инициализируем пустым списком
            }
            return comments;
        }

        // GET: Comment/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return View(comment);
        }

        // GET: Comment/Create
        public IActionResult Create(int articleId)
        {
            // Передаем ID статьи в представление, чтобы знать, к какой статье добавлять комментарий
            ViewBag.ArticleId = articleId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int articleId, string content)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest("Invalid User ID format");
            }

            var article = await _context.Articles.FindAsync(articleId);
            if (article == null)
            {
                return NotFound("Article not found");
            }

            var comment = new Comment
            {
                ArticleId = articleId,
                UserId = userId,
                Content = content,
                CommentDate = DateTime.UtcNow
            };

            await _commentService.CreateCommentAsync(comment);

            TempData["IncreaseViewCount"] = false;

            //// Перенаправление на страницу статьи, но не вызываем метод Details
            return RedirectToAction("Details", "Article", new { id = articleId, isView = false } );
        }

        // GET: Comment/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return View(comment);
        }

        // POST: Comment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CommentId,ArticleId,UserId,Content,CommentDate")] Comment comment)
        {
            Console.WriteLine(id);
            Console.WriteLine(comment.CommentId);
            Console.WriteLine(comment.ArticleId); // Для проверки
            Console.WriteLine(comment.UserId); // Для проверки

            if (id != comment.CommentId)
            {
                return NotFound();
            }

            
                try
                {
                    await _commentService.UpdateCommentAsync(comment);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.CommentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Comments", "Home");
                
            
        }

        // GET: Comment/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _commentService.DeleteCommentAsync(id);
            return RedirectToAction("Comments", "Home");
        }

        
  

        private bool CommentExists(int id)
        {
            return _commentService.GetCommentByIdAsync(id) != null;
        }
    }
}
