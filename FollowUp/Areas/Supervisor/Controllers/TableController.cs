using FollowUp.Data;
using FollowUp.Data.Services;
using FollowUp.Models;
using FollowUp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace FollowUp.Areas.Supervisor.Controllers
{
    [Authorize(Roles = StaticDetails.Supervisor)]
    [Area(nameof(Supervisor))]
    [Route(nameof(Supervisor) + "/[controller]/[action]")]
    public class TableController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailProvider _emailProvider;
        public TableController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment
            , UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailProvider emailProvider)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailProvider = emailProvider;
        }

        public async Task<IActionResult> Index(string? selectedDate)
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

            var daysInArabic = new Dictionary<DayOfWeek, string>
            {
                { DayOfWeek.Saturday, "السبت" },
                { DayOfWeek.Sunday, "الأحد" },
                { DayOfWeek.Monday, "الاثنين" },
                { DayOfWeek.Tuesday, "الثلاثاء" },
                { DayOfWeek.Wednesday, "الأربعاء" },
                { DayOfWeek.Thursday, "الخميس" },
                { DayOfWeek.Friday, "الجمعة" }
            };

            var todayInArabic = daysInArabic[DateTime.Today.DayOfWeek];

            var Course = _context.Tables
                .Include(x => x.ApplicationUser)
                .Include(y => y.Department)
                .Include(y => y.Build)
                .Include(y => y.Course)
                .AsQueryable();

            var dates = await _context.Tables
                .Select(a => a.Day)
                .Distinct()
                .ToListAsync();

            if (selectedDate == "كل الايام")
            {
                Course = Course.Where(r => r.Activation.Status == "نشط" && (r.TypeDivition == "نظري صباحي" || r.TypeDivition == "عملي صباحي"));
            }
            else if (selectedDate != null)
            {
                Course = Course.Where(a => a.Day == selectedDate && a.Activation.Status == "نشط" && (a.TypeDivition == "نظري صباحي" || a.TypeDivition == "عملي صباحي"));
            }
            else
            {
                Course = Course.Where(a => a.Day == todayInArabic && a.Activation.Status == "نشط" && (a.TypeDivition == "نظري صباحي" || a.TypeDivition == "عملي صباحي"));
            }

            var attendances = await Course.ToListAsync();

            ViewBag.Dates = dates;
            ViewBag.SelectedDate = selectedDate ?? todayInArabic;

            var today = DateTime.Today;

            var results = await _context.Attendances
                .Where(r => r.Date.Date == today)
                .ToListAsync();

            ViewBag.Results = results;

            return View(Course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Attendance(int id, string value, int? minutes)
        {
            try
            {
                var table = await _context.Tables.FindAsync(id);
                var user = await _context.ApplicationUsers.FindAsync(table.TrainerId);

                var today = DateTime.Today;

                var results = await _context.Attendances
                    .Where(r => r.Date.Date == today && r.TableId == table.Id && r.ApplicationUser.Id == user.Id)
                    .CountAsync();

                if (results != 0)
                {
                    return RedirectToAction("Index");
                }

                if (value == "غائب")
                {
                    var attendance = new Attendance
                    {
                        Value = "غائب",
                        Date = DateTime.Now,
                        TableId = id,
                        TrainerId = table.TrainerId,
                        ApplicationUser = user,
                    };

                    _context.Attendances.Add(attendance);
                    await _context.SaveChangesAsync();

                    await _emailProvider.SendMail( table.Id, user.Id, value, minutes);

                    HttpContext.Session.SetString("created", "true");
                    return RedirectToAction("Index");
                }
                else if (value == "متأخر" && minutes.HasValue)
                {
                    var attendance = new Attendance
                    {
                        Value = "متأخر",
                        Minutes = minutes.Value,
                        Date = DateTime.Now,
                        TableId = id,
                        TrainerId = table.TrainerId,
                        ApplicationUser = user,
                    };

                    _context.Attendances.Add(attendance);
                    await _context.SaveChangesAsync();

                    await _emailProvider.SendMail( table.Id, user.Id, value, minutes);

                    HttpContext.Session.SetString("updated", "true");
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

    }
}
