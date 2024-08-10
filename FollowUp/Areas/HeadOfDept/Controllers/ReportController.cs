using FollowUp.Data.Services;
using FollowUp.Data;
using FollowUp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace FollowUp.Areas.HeadOfDept.Controllers
{
    [Authorize(Roles = StaticDetails.HeadOfDept)]
    [Area(nameof(HeadOfDept))]
    [Route(nameof(HeadOfDept) + "/[controller]/[action]")]
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
            var Tra = await _context.ApplicationUsers.Include(x => x.Department).FirstOrDefaultAsync(v => v.Id == userId);

            var Course = await _context.Attendances
                .Include(a => a.Table)
                    .ThenInclude(t => t.Department)
                    .Include(a => a.Table)
                    .ThenInclude(d => d.Build)
                    .Include(a => a.Table)
                    .ThenInclude(b => b.Course)
                .Include(a => a.ApplicationUser)
                .Where(x => x.ApplicationUser.Department.Name == Tra.Department.Name)
                .ToListAsync();

            return View(Course);
        }


        public async Task<IActionResult> SendOne(int id)
        {
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
        public async Task<IActionResult> FirstExcuse(int id, IFormFile HeadOfDeptSignture, string FirstAnswer)
        {
            var department = await _context.Attendances.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _context.ApplicationUsers.Include(x => x.Department).FirstOrDefaultAsync(v => v.Id == userId);

                var today = DateTime.Today;
                var hijriCalendar = new System.Globalization.HijriCalendar();
                var hijriDate = $"{hijriCalendar.GetDayOfMonth(today)}/{hijriCalendar.GetMonth(today)}/{hijriCalendar.GetYear(today)}";

                department.FirstAnswer = FirstAnswer;
                department.Status = 3;
                department.HeadOfDeptName = user.UserFullName;
                department.HeadOfDeptSendDate = hijriDate;

                if (HeadOfDeptSignture != null)
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Signture");

                    string[] allowedExtensions = { ".png", ".jpg", ".jpeg" };
                    if (!allowedExtensions.Contains(Path.GetExtension(HeadOfDeptSignture.FileName).ToLower()))
                    {
                        TempData["ErrorMessage"] = "Only .png and .jpg and .jpeg images are allowed!";
                        return RedirectToAction("Create");
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + HeadOfDeptSignture.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    await using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await HeadOfDeptSignture.CopyToAsync(fileStream);
                    }
                    department.HeadOfDeptSignture = uniqueFileName;
                }

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FirstExcuseHead(int id, string Statment)
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

        public async Task<IActionResult> DeActivate(int? Id)
        {
            try
            {
                if (Id == null)
                    return NotFound();
                var service = await _context.Attendances.FirstOrDefaultAsync(m => m.Id == Id);
                if (service == null)
                    return NotFound();

                service.Status = 1;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }
        public async Task<IActionResult> Activate(int? Id)
        {
            try
            {
                if (Id == null)
                    return NotFound();
                var service = await _context.Attendances.FirstOrDefaultAsync(m => m.Id == Id);
                if (service == null)
                    return NotFound();

                service.Status = 5;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
