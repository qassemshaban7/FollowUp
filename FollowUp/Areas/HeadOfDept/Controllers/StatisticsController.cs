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
        public async Task<IActionResult> Statistics(int? day, int? month, int? year)
        {
            HijriCalendar hijriCalendar = new HijriCalendar();

            if (month == null || year == null)
            {
                ViewBag.PresentStatistics = 0;
                ViewBag.LateStatistics = 0;
                ViewBag.AbsentStatistics = 0;
                ViewBag.TotalTables = 0;

                return View();

                //DateTime now = DateTime.Now;
                //month = month ?? hijriCalendar.GetMonth(now);
                //year = year ?? hijriCalendar.GetYear(now);
            }

            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _context.ApplicationUsers.Include(x => x.Department).FirstOrDefaultAsync(v => v.Id == userId);

            if (day != null)
            {
                try
                {
                    DateTime hijriDate = hijriCalendar.ToDateTime(year.Value, month.Value, day.Value, 0, 0, 0, 0);

                    CultureInfo arabicCulture = new CultureInfo("ar-SA");
                    string dayNameInArabic = arabicCulture.DateTimeFormat.GetDayName(hijriDate.DayOfWeek);

                    var Absent = await _context.Attendances
                        .Where(x => x.ApplicationUser.Department.Name == user.Department.Name && x.HijriDate == $"{day}/{month}/{year}" && x.Value == "متأخر")
                        .CountAsync();

                    var Late = await _context.Attendances
                        .Where(x => x.ApplicationUser.Department.Name == user.Department.Name && x.HijriDate == $"{day}/{month}/{year}" && x.Value == "غائب")
                        .CountAsync();

                    var Tables = await _context.Tables
                        .Where(r => r.ApplicationUser.Department.Name == user.Department.Name && r.Activation.Status == "نشط" && r.Day == dayNameInArabic)
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
            }
            else
            {
                try
                {
                    int daysInMonth = hijriCalendar.GetDaysInMonth(year.Value, month.Value);

                    int totalAbsent = 0;
                    int totalLate = 0;
                    int totalTables = 0;

                    for (int d = 1; d <= daysInMonth; d++)
                    {
                        DateTime hijriDate = hijriCalendar.ToDateTime(year.Value, month.Value, d, 0, 0, 0, 0);
                        string dayNameInArabic = new CultureInfo("ar-SA").DateTimeFormat.GetDayName(hijriDate.DayOfWeek);

                        var dailyAbsent = await _context.Attendances
                            .Where(x => x.ApplicationUser.Department.Name == user.Department.Name && x.HijriDate == $"{d}/{month}/{year}" && x.Value == "متأخر")
                            .CountAsync();

                        var dailyLate = await _context.Attendances
                            .Where(x => x.ApplicationUser.Department.Name == user.Department.Name && x.HijriDate == $"{d}/{month}/{year}" && x.Value == "غائب")
                            .CountAsync();

                        var dailyTables = await _context.Tables
                            .Where(r => r.ApplicationUser.Department.Name == user.Department.Name && r.Activation.Status == "نشط" && r.Day == dayNameInArabic)
                            .CountAsync();

                        totalAbsent += dailyAbsent;
                        totalLate += dailyLate;
                        totalTables += dailyTables;
                    }

                    ViewBag.totalTables = totalTables;

                    if (totalTables > 0)
                    {
                        var PresentStatistics = (totalTables - (totalAbsent + totalLate));
                        ViewBag.PresentStatistics = PresentStatistics;

                        var LateStatistics = totalLate;
                        ViewBag.LateStatistics = LateStatistics;

                        var AbsentStatistics = totalAbsent;
                        ViewBag.AbsentStatistics = AbsentStatistics;
                    }
                    else
                    {
                        ViewBag.PresentStatistics = 0;
                        ViewBag.LateStatistics = 0;
                        ViewBag.AbsentStatistics = 0;
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    ViewBag.PresentStatistics = 0;
                    ViewBag.LateStatistics = 0;
                    ViewBag.AbsentStatistics = 0;
                }
            }

            return View();
        }
    }
}
