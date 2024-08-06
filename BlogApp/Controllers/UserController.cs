using BlogApp.Models.Services;
using BlogApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BlogApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public UserController(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            
            return View(users);
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,FirstName, LastName,Email,Phone,RegistrationDate,RoleId,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                await _userService.CreateUserAsync(user);
                return RedirectToAction("Users", "Home");
            }
            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Получаем список ролей
            var roles = await _roleService.GetAllRolesAsync();

            var viewModel = new UserEditViewModel
            {
                User = user,
                Roles = roles, // Обязательно загружаем роли
                SelectedRoleId = user.RoleId
            };

            return View(viewModel);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserEditViewModel model)
        {
            if (id != model.User.UserId)
            {
                return NotFound();
            }

            Console.WriteLine($"SelectedRoleId: {model.SelectedRoleId}");

            model.Roles = await _roleService.GetAllRolesAsync();
            // Проверьте список доступных ролей
            Console.WriteLine("Available Roles:");
            foreach (var role in model.Roles)
            {
                Console.WriteLine($"RoleId: {role.RoleId}, Name: {role.Name}");
            }

            var selectedRole = model.Roles.FirstOrDefault(r => r.RoleId == model.SelectedRoleId);

            if (selectedRole == null)
            {
                ModelState.AddModelError("SelectedRoleId", "The selected role is invalid.");
                return View(model);
            }

            
                try
                {
                    // Обновляем данные пользователя
                    model.User.RoleId = selectedRole.RoleId;
                    model.User.Role = selectedRole;
                    await _userService.UpdateUserAsync(model.User);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await UserExistsAsync(model.User.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }           

            model.Roles = await _roleService.GetAllRolesAsync();
            return RedirectToAction("Users", "Home");
        }

        private async Task<bool> UserExistsAsync(int id)
        {
            return await _userService.GetUserByIdAsync(id) != null;
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userService.DeleteUserAsync(id);
            return RedirectToAction("Users", "Home");
        }

        private bool UserExists(int id)
        {
            return _userService.GetUserByIdAsync(id) != null;
        }
    }
}
