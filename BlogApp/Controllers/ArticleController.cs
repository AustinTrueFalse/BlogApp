using BlogApp.Models.Services;
using BlogApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlogApp.Controllers
{
    [Authorize]
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ApplicationDbContext _context;

        public ArticleController(IArticleService articleService, ApplicationDbContext context)
        {
            _articleService = articleService;
            _context = context;
        }

        // GET: Article
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            /// Выводим в консоль роль человека вызывающего метод

            Console.WriteLine(role);

            if (role == "Administrator")
            {
                var articles = await _articleService.GetAllArticlesAsync();
                return View(articles);

                
            }
            else
            {
                var articles = await _articleService.GetArticlesByAuthorIdAsync(int.Parse(userId));
                return View(articles);
            }
        }

        // GET: Article/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            var role = User.FindFirstValue(ClaimTypes.Role);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (role != "Administrator" && article.UserId.ToString() != userId)
            {
                return Forbid();
            }

            return View(article);
        }

        // GET: Article/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Article/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content")] Article article)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                article.UserId = userId;
                await _articleService.CreateArticleAsync(article);
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        // GET: Article/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role != "Administrator" && article.UserId.ToString() != userId)
            {
                return Forbid();
            }

            return View(article);
        }

        // POST: Article/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticleId,Title,Content")] Article article)
        {
            if (id != article.ArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingArticle = await _articleService.GetArticleByIdAsync(id);
                    if (existingArticle == null)
                    {
                        return NotFound();
                    }

                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var role = User.FindFirstValue(ClaimTypes.Role);

                    if (role != "Administrator" && existingArticle.UserId.ToString() != userId)
                    {
                        return Forbid();
                    }

                    existingArticle.Title = article.Title;
                    existingArticle.Content = article.Content;

                    await _articleService.UpdateArticleAsync(existingArticle);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ArticleExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        // GET: Article/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            var role = User.FindFirstValue(ClaimTypes.Role);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (role != "Administrator" && article.UserId.ToString() != userId)
            {
                return Forbid();
            }

            return View(article);
        }

        // POST: Article/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _articleService.DeleteArticleAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ArticleExists(int id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            return article != null;
        }
    }
}