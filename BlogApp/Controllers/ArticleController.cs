using BlogApp.Models.Services;
using BlogApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlogApp.Controllers
{
    [Authorize]
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IUserService _userService;
        private readonly ITagService _tagService;
        private readonly ApplicationDbContext _context;

        public ArticleController(IArticleService articleService, IUserService userService, ITagService tagService, ApplicationDbContext context)
        {
            _articleService = articleService;
            _userService = userService;
            _tagService = tagService;
            _context = context;
        }

        // GET: Article/UserArticles
        public async Task<IActionResult> UserArticles()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(); // Или перенаправление на страницу входа
            }

            var articles = await _articleService.GetArticlesByAuthorIdAsync(int.Parse(userId));
            return View(articles);
        }

        // GET: Article
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            /// Выводим в консоль роль человека вызывающего метод

           

            var articles = await _articleService.GetAllArticlesAsync();
            return View(articles);

        }

        // GET: Article/Details/5
        public async Task<IActionResult> Details(int id, bool isView = true)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            // Увеличиваем счетчик просмотров только если это обычный просмотр
            if (isView)
            {
                article.ViewCount++;
                await _articleService.UpdateArticleAsync(article);
            }

            return View(article);
        }

        // GET: Article/Create
        public async Task<IActionResult> Create()
        {
            var tags = await _tagService.GetAllTagsAsync();
            var model = new ArticleViewModel
            {
                AvailableTags = tags.Select(tag => new TagViewModel
                {
                    Id = tag.TagId,
                    Name = tag.Name
                }).ToList()
            };
            return View(model);
        }

        // POST: Article/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(); // Или перенаправление на страницу входа
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            


            if (string.IsNullOrEmpty(userIdClaim))
            {
                return BadRequest("User ID not found in claims");
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest("Invalid User ID format");
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }


            if (ModelState.IsValid)
            {
                var article = new Article
                {
                    Title = model.Title,
                    Content = model.Content,
                    UserId = user.UserId
                };

                if (model.SelectedTags != null && model.SelectedTags.Any())
                {
                    var _tags = await _context.Tags.Where(tag => model.SelectedTags.Contains(tag.TagId)).ToListAsync();
                    article.Tags = _tags;
                }

                await _articleService.CreateArticleAsync(article);
                return RedirectToAction("Articles", "Home");
            }




            var tags = await _tagService.GetAllTagsAsync();
            model.AvailableTags = tags.Select(tag => new TagViewModel
            {
                Id = tag.TagId,
                Name = tag.Name
            }).ToList();
            return View(model);
        }

        // GET: Article/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            var tags = await _tagService.GetAllTagsAsync();
            if (tags == null)
            {
                tags = new List<Tag>(); // Предотвращаем ошибку, если tags равен null
            }


            var model = new ArticleViewModel
            {
                ArticleId = id,
                Title = article.Title,
                Content = article.Content,
                SelectedTags = article.Tags?.Select(t => t.TagId).ToList() ?? new List<int>(),
                AvailableTags = tags.Select(tag => new TagViewModel
                {
                    Id = tag.TagId,
                    Name = tag.Name,
                    IsSelected = article.Tags?.Any(t => t.TagId == tag.TagId) ?? false
                }).ToList()
            };

            return View(model);
        }

        // POST: Article/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ArticleViewModel model)
        {
            Console.WriteLine("Пошел пост");

            if (id != model.ArticleId)
            {
                return NotFound();
            }
            Console.WriteLine("Айди статьи");
            Console.WriteLine(model.ArticleId);

            if (ModelState.IsValid)
            {
                try
                {
                    var existingArticle = await _articleService.GetArticleByIdAsync(id);
                    if (existingArticle == null)
                    {
                        return NotFound();
                    }

                    existingArticle.Title = model.Title;
                    existingArticle.Content = model.Content;

                    var selectedTags = model.SelectedTags ?? new List<int>();
                    var tags = await _context.Tags.Where(t => selectedTags.Contains(t.TagId)).ToListAsync();

                    existingArticle.Tags = tags;

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
                return RedirectToAction("Articles", "Home");
            }

            // Re-populate the model's tags for the view if ModelState is invalid
            var allTags = await _tagService.GetAllTagsAsync();
            model.AvailableTags = allTags.Select(tag => new TagViewModel
            {
                Id = tag.TagId,
                Name = tag.Name,
                IsSelected = model.SelectedTags.Contains(tag.TagId)
            }).ToList();

            return View(model);
        }

        // GET: Article/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            return RedirectToAction("DeleteConfirmed", new { id });
        }

        // POST: Article/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _articleService.DeleteArticleAsync(id);
            return RedirectToAction("Articles", "Home");
        }

        private async Task<bool> ArticleExists(int id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            return article != null;
        }
    }
}