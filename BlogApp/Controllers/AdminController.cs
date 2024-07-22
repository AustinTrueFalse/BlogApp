using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Пример действия, доступного только для администратора
        public IActionResult Index()
        {
            return View();
        }

        // Другие действия
    }

    [Authorize(Policy = "ModeratorOnly")]
    public class ModeratorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModeratorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Пример действия, доступного только для модераторов
        public IActionResult Index()
        {
            return View();
        }

        // Другие действия
    }

    [Authorize(Policy = "UserOnly")]
    public class DefaultUserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DefaultUserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Пример действия, доступного только для пользователей
        public IActionResult Index()
        {
            return View();
        }

        // Другие действия
    }
}
