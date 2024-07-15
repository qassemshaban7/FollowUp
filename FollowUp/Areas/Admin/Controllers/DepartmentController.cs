using FollowUp.Data;
using FollowUp.Models;
using FollowUp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FollowUp.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.Admin)]
    [Area(nameof(Admin))]
    [Route(nameof(Admin) + "/[controller]/[action]")]
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public DepartmentController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment
            , UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {

            if (HttpContext.Session.GetString("created") != null)
            {
                ViewBag.created = true;
                HttpContext.Session.Remove("created");
            }
            if (HttpContext.Session.GetString("updated") != null)
            {
                ViewBag.updated = true;
                HttpContext.Session.Remove("updated");
            }
            if (HttpContext.Session.GetString("deleted") != null)
            {
                ViewBag.deleted = true;
                HttpContext.Session.Remove("deleted");
            }

            var Course = await _context.Departments.ToListAsync();

            return View(Course);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Departments"] = new SelectList(await _context.Departments.ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Departments.Add(department);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("created", "true");
                return RedirectToAction("Index");
            }
            return View(department);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Departments.Update(department);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("updated", "true");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    return RedirectToAction(nameof(Index));
                }

            }
            return View(department);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ApplicationUsers = _context.ApplicationUsers.Where(rs => rs.DepartmentId == id).ToList();
            _context.ApplicationUsers.RemoveRange(ApplicationUsers);
            await _context.SaveChangesAsync();

            var course = await _context.Departments.FindAsync(id);
            _context.Departments.Remove(course);
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("deleted", "true");
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}
