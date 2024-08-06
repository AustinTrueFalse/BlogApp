using BlogApp.Models;
using BlogApp.Models.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BlogApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly ITagService _tagService;
        private readonly IArticleService _articleService;
        private readonly ICommentService _commentService;
        private readonly ApplicationDbContext _context;

        public HomeController(
            ILogger<HomeController> logger, 
            IUserService userService,
            IRoleService roleService,
            ITagService tagService,
            IArticleService articleService,
            ICommentService commentService,
            ApplicationDbContext context
            )

        {
            _logger = logger;
            _userService = userService;
            _roleService = roleService;
            _tagService = tagService;
            _articleService = articleService;
            _commentService = commentService;
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Articles");
        }

        public async Task<IActionResult> Comments()
        {
            var comments = await _commentService.GetAllCommentsAsync();
            return View(comments);
        }

        // GET: Home/Tags
        public async Task<IActionResult> Tags()
        {
            var tags = await _tagService.GetAllTagsAsync();
            var tagsWithArticleCount = tags.Select(tag => new
            {
                Tag = tag,
                ArticleCount = _context.Articles.Count(a => a.Tags.Any(t => t.TagId == tag.TagId))
            }).ToList();

            return View(tagsWithArticleCount);
        }

        // GET: Home/Users
        public async Task<IActionResult> Users()
        {
            var users = await _userService.GetAllUsersAsync();
            foreach (var user in users)
            {
                user.Role = await _roleService.GetRoleByIdAsync(user.RoleId); // Получаем роль для пользователя
            }
            return View(users);
        }

        public async Task<IActionResult> Roles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return View(roles);
        }

        // GET: Home/Articles
        public async Task<IActionResult> Articles()
        {
            var articles = await _articleService.GetAllArticlesAsync();
            return View(articles);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult NotFound()
        {
            return View();
        }
    }
}
