using FollowUp.Data.Services;
using FollowUp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FollowUp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using FollowUp.Models;
using System.Security.Claims;

namespace FollowUp.Areas.Trainer.Controllers
{
    [Authorize(Roles = StaticDetails.Trainer)]
    [Area(nameof(Trainer))]
    [Route(nameof(Trainer) + "/[controller]/[action]")]
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
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var Course = await _context.Attendances
                .Include(a => a.Table)
                    .ThenInclude(t => t.Department)
                    .Include(a => a.Table)
                    .ThenInclude(d => d.Build)
                    .Include(a => a.Table)
                    .ThenInclude(b => b.Course)
                .Include(a => a.ApplicationUser)
                .Where(c => c.ApplicationUser.Id == userId)
                .ToListAsync();


            return View(Course);
        }

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
        public async Task<IActionResult> FirstExcuse(int id, string Statment)
        {
            var department = await _context.Attendances.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            try
            {
                var today = DateTime.Today;
                var hijriCalendar = new System.Globalization.HijriCalendar();
                var hijriDate = $"{hijriCalendar.GetDayOfMonth(today)}/{hijriCalendar.GetMonth(today)}/{hijriCalendar.GetYear(today)}";

                department.Statment = Statment;
                department.Status = 2;
                department.StatmentDate = hijriDate;

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
