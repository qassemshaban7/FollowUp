using FollowUp.Data.Services;
using FollowUp.Data;
using FollowUp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FollowUp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FollowUp.Areas.Supervisor.Controllers
{
    [Authorize(Roles = StaticDetails.Supervisor)]
    [Area(nameof(Supervisor))]
    [Route(nameof(Supervisor) + "/[controller]/[action]")]
    public class PermissionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailProvider _emailProvider;
        public PermissionController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment
            , UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailProvider emailProvider)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailProvider = emailProvider;
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
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var Course = await _context.Permissions
                .Where(c => c.TrainerId == userId)
                .ToListAsync();

            return View(Course);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string? returnUrl = null)
        {
            var manager3 = await _context.configs.Where(x => x.Id == 3).FirstOrDefaultAsync();
            ViewBag.manager3 = manager3;

            var manager4 = await _context.configs.Where(x => x.Id == 4).FirstOrDefaultAsync();
            ViewBag.manager4 = manager4;

            var manager5 = await _context.configs.Where(x => x.Id == 5).FirstOrDefaultAsync();
            ViewBag.manager5 = manager5;

            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _context.ApplicationUsers.Include(x => x.Department).FirstOrDefaultAsync(v => v.Id == userId);
            ViewBag.user = user;

            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Permissions"] = new SelectList(await _context.Permissions.ToListAsync(), "Id", "Value", "to", "fromdate");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TimeOnly fromdate, TimeOnly to, DateTime Date, string Value, Permission department)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _context.ApplicationUsers.Include(x => x.Department).FirstOrDefaultAsync(v => v.Id == userId);

                department.Date = Date;
                department.fromdate = fromdate;
                department.to = to;
                department.TrainerId = user.Id;
                department.ApplicationUser = user;
                department.Value = Value;
                department.Status = 1;
                _context.Permissions.Add(department);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("created", "true");
                return RedirectToAction("Index");
            }
            catch
            {
                return View(department);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var manager3 = await _context.configs.Where(x => x.Id == 3).FirstOrDefaultAsync();
            ViewBag.manager3 = manager3;

            var manager4 = await _context.configs.Where(x => x.Id == 4).FirstOrDefaultAsync();
            ViewBag.manager4 = manager4;

            var manager5 = await _context.configs.Where(x => x.Id == 5).FirstOrDefaultAsync();
            ViewBag.manager5 = manager5;

            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Permissions.Include(x => x.ApplicationUser).ThenInclude(z => z.Department).FirstOrDefaultAsync(e => e.Id == id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TimeOnly fromdate, TimeOnly to, DateTime Date, int id, string Value)
        {
            var department = await _context.Permissions.FirstOrDefaultAsync(v => v.Id == id);
            if (id != department.Id)
            {
                return NotFound();
            }

            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _context.ApplicationUsers.Include(x => x.Department).FirstOrDefaultAsync(v => v.Id == userId);

                department.TrainerId = user.Id;
                department.ApplicationUser = user;
                department.Value = Value;
                department.Date = Date;
                department.fromdate = fromdate;
                department.to = to;
                department.Status = 1;
                _context.Permissions.Update(department);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("updated", "true");
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return View(department);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dept = await _context.Permissions.FindAsync(id);
            _context.Permissions.Remove(dept);
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("deleted", "true");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Report(int id)
        {
            var manager = await _context.configs.Where(x => x.Id == 1).FirstOrDefaultAsync();
            var manager2 = await _context.configs.Where(x => x.Id == 2).FirstOrDefaultAsync();
            ViewBag.manager = manager;
            ViewBag.manager2 = manager2;

            var manager3 = await _context.configs.Where(x => x.Id == 3).FirstOrDefaultAsync();
            ViewBag.manager3 = manager3;

            var manager4 = await _context.configs.Where(x => x.Id == 4).FirstOrDefaultAsync();
            ViewBag.manager4 = manager4;

            var manager5 = await _context.configs.Where(x => x.Id == 5).FirstOrDefaultAsync();
            ViewBag.manager5 = manager5;

            var repo = await _context.Permissions.Include(x => x.ApplicationUser).ThenInclude(v => v.Department).FirstOrDefaultAsync(c => c.Id == id);
            return View(repo);
        }
    }
}
