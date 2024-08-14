using FollowUp.Data.Services;
using FollowUp.Data;
using FollowUp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace FollowUp.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.Admin)]
    [Area(nameof(Admin))]
    [Route(nameof(Admin) + "/[controller]/[action]")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailProvider _emailProvider;
        public ReportController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment
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

            var Course = await _context.Attendances
                .Include(a => a.Table)
                    .ThenInclude(t => t.Department)
                    .Include(a => a.Table)
                    .ThenInclude(d => d.Build)
                    .Include(a => a.Table)
                    .ThenInclude(b => b.Course)
                .Include(a => a.ApplicationUser)
                .ToListAsync();

            return View(Course);
        }


        public async Task<IActionResult> SendOne(int id)
        {
            var manager3 = await _context.configs.Where(x => x.Id == 3).FirstOrDefaultAsync();
            ViewBag.manager3 = manager3;

            var manager4 = await _context.configs.Where(x => x.Id == 4).FirstOrDefaultAsync();
            ViewBag.manager4 = manager4;

            var manager5 = await _context.configs.Where(x => x.Id == 5).FirstOrDefaultAsync();
            ViewBag.manager5 = manager5;

            var Attendance = await _context.Attendances
                .Include(a => a.Table)
                    .ThenInclude(t => t.Department)
                .Include(a => a.Table)
                    .ThenInclude(d => d.Build)
                .Include(a => a.Table)
                    .ThenInclude(b => b.Course)
                .Include(a => a.ApplicationUser)
                    .ThenInclude(t => t.Department)
                .FirstOrDefaultAsync(r => r.Id == id);

            return View(Attendance);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FirstExcuse(int id, string SecondAnswer)
        {
            var department = await _context.Attendances.FindAsync(id);
            if (department == null) return NotFound();

            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _context.ApplicationUsers.Include(x => x.Department).FirstOrDefaultAsync(v => v.Id == userId);

                department.SecondAnswer = SecondAnswer;
                department.Status = 4;
                department.DeanName = user.UserFullName;

                _context.Attendances.Update(department);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("updated", "true");
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return View(department);
            }
        }
    }
}
