using BlogApp.Models.Services;
using BlogApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // GET: Role
        public async Task<IActionResult> Index()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return View(roles);
        }

        // GET: Role/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // GET: Role/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoleId,Name,Description")] Role role)
        {
            if (ModelState.IsValid)
            {
                await _roleService.CreateRoleAsync(role);
                return RedirectToAction("Roles", "Home");
            }
            // Debugging output
            foreach (var state in ModelState)
            {
                if (state.Value.Errors.Count > 0)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
                    }
                }
            }

            Console.WriteLine("Роль не валидна");
            return View(role);
        }

        // GET: Role/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: Role/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoleId,Name,Description")] Role role)
        {
            if (id != role.RoleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _roleService.UpdateRoleAsync(role);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleExists(role.RoleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Roles", "Home");
            }
            return View(role);
        }

        // GET: Role/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Role/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _roleService.DeleteRoleAsync(id);
            return RedirectToAction("Roles", "Home");
        }

        private bool RoleExists(int id)
        {
            return _roleService.GetRoleByIdAsync(id) != null;
        }
    }
}
