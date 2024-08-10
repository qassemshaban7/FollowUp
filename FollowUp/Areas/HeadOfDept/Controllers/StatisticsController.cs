using FollowUp.Data;
using FollowUp.Models;
using FollowUp.Utility;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;
using System.Security.Claims;

namespace FollowUp.Areas.HeadOfDept.Controllers
{
    [Authorize(Roles = StaticDetails.HeadOfDept)]
    [Area(nameof(HeadOfDept))]
    [Route(nameof(HeadOfDept) + "/[controller]/[action]")]
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Statistics(DateOnly? date)
        {
            HijriCalendar hijriCalendar = new HijriCalendar();

            if (date == null)
            {
                ViewBag.PresentStatistics = 0;
                ViewBag.LateStatistics = 0;
                ViewBag.AbsentStatistics = 0;
                ViewBag.TotalTables = 0;
                return View();
            }
            ViewData["SelectedDate"] = date.Value.ToString("yyyy-MM-dd");

            try
            {
                DateTime dateTime = date.Value.ToDateTime(new TimeOnly(0, 0));
                int day = hijriCalendar.GetDayOfMonth(dateTime);
                int month = hijriCalendar.GetMonth(dateTime);
                int year = hijriCalendar.GetYear(dateTime);

                CultureInfo arabicCulture = new CultureInfo("ar-SA");
                string dayNameInArabic = arabicCulture.DateTimeFormat.GetDayName(dateTime.DayOfWeek);

                var Absent = await _context.Attendances
                    .Where(x => x.HijriDate == $"{day}/{month}/{year}" && x.Value == "غائب")
                    .CountAsync();
                
                var Late = await _context.Attendances
                    .Where(x => x.HijriDate == $"{day}/{month}/{year}" && x.Value == "متأخر")
                    .CountAsync();

                var Tables = await _context.Tables
                    .Where(r => r.Activation.Status == "نشط" && r.Day == dayNameInArabic)
                    .CountAsync();
                ViewBag.TotalTables = Tables;

                var PresentStatistics = Tables - (Absent + Late);
                ViewBag.PresentStatistics = PresentStatistics;

                var LateStatistics = Late;
                ViewBag.LateStatistics = LateStatistics;

                var AbsentStatistics = Absent;
                ViewBag.AbsentStatistics = AbsentStatistics;
            }
            catch (ArgumentOutOfRangeException)
            {
                ViewBag.PresentStatistics = 0;
                ViewBag.LateStatistics = 0;
                ViewBag.AbsentStatistics = 0;
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Absent(DateOnly? date)
        {
            if (!date.HasValue)
            {
                return View(new List<Attendance>());
            }
            HijriCalendar hijriCalendar = new HijriCalendar();

            DateTime dateTime = date.Value.ToDateTime(new TimeOnly(0, 0));
            int day = hijriCalendar.GetDayOfMonth(dateTime);
            int month = hijriCalendar.GetMonth(dateTime);
            int year = hijriCalendar.GetYear(dateTime);


            var Absent = await _context.Attendances
                .Where(x => x.HijriDate == $"{day}/{month}/{year}" && x.Value == "غائب")
                .Include(a => a.Table)
                    .ThenInclude(t => t.Department)
                    .Include(a => a.Table)
                    .ThenInclude(d => d.Build)
                    .Include(a => a.Table)
                    .ThenInclude(b => b.Course)
                .Include(a => a.ApplicationUser)
                .ToListAsync();

            return View(Absent);
        }

        [HttpGet]
        public async Task<IActionResult> Late(DateOnly? date)
        {
            if (!date.HasValue)
            {
                return View(new List<Attendance>());
            }
            HijriCalendar hijriCalendar = new HijriCalendar();

            DateTime dateTime = date.Value.ToDateTime(new TimeOnly(0, 0));
            int day = hijriCalendar.GetDayOfMonth(dateTime);
            int month = hijriCalendar.GetMonth(dateTime);
            int year = hijriCalendar.GetYear(dateTime);

            var Late = await _context.Attendances
                .Where(x => x.HijriDate == $"{day}/{month}/{year}" && x.Value == "متأخر")
                .Include(a => a.Table)
                    .ThenInclude(t => t.Department)
                    .Include(a => a.Table)
                    .ThenInclude(d => d.Build)
                    .Include(a => a.Table)
                    .ThenInclude(b => b.Course)
                .Include(a => a.ApplicationUser)
                .ToListAsync();

            return View(Late);
        }

    }
}
