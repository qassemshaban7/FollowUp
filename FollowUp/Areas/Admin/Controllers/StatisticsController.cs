using FollowUp.Data;
using FollowUp.Models;
using FollowUp.Utility;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;

namespace FollowUp.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.Admin)]
    [Area(nameof(Admin))]
    [Route(nameof(Admin) + "/[controller]/[action]")]
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

            if (day != null)
            {
                try
                {
                    DateTime hijriDate = hijriCalendar.ToDateTime(year.Value, month.Value, day.Value, 0, 0, 0, 0);

                    CultureInfo arabicCulture = new CultureInfo("ar-SA");
                    string dayNameInArabic = arabicCulture.DateTimeFormat.GetDayName(hijriDate.DayOfWeek);

                    var Absent = await _context.Attendances
                        .Where(x => x.HijriDate == $"{day}/{month}/{year}" && x.Value == "متأخر")
                        .CountAsync();

                    var Late = await _context.Attendances
                        .Where(x => x.HijriDate == $"{day}/{month}/{year}" && x.Value == "غائب")
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
                            .Where(x => x.HijriDate == $"{d}/{month}/{year}" && x.Value == "متأخر")
                            .CountAsync();

                        var dailyLate = await _context.Attendances
                            .Where(x => x.HijriDate == $"{d}/{month}/{year}" && x.Value == "غائب")
                            .CountAsync();

                        var dailyTables = await _context.Tables
                            .Where(r => r.Activation.Status == "نشط" && r.Day == dayNameInArabic)
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

        //[HttpGet("startbackgroundTasktoAddAttendance")]
        //public IActionResult StartBackgroundTaskToAddAttendance()
        //{
        //    RecurringJob.AddOrUpdate(() => AddAttendance(), Cron.Minutely);

        //    return Ok("Background Tasks Started to Add Attendance");
        //}


        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task AddAttendance()
        //{
        //    var yesterday = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

        //    if (yesterday.DayOfWeek == DayOfWeek.Friday && yesterday.DayOfWeek == DayOfWeek.Sunday)
        //    {
        //        return;
        //    }

        //    var user = await _context.ApplicationUsers.FindAsync("8d152e07 - 2ab0 - 4f5c - b11a - 645488975f8c");

        //    var attendance = new Attendance
        //    {
        //        Value = "غائب",
        //        Date = DateTime.Now,
        //        HijriDate = "11",
        //        TableId = 242,
        //        TrainerId = user.Id,
        //        ApplicationUser = user,
        //        Status = 1,
        //    };
        //    _context.Attendances.Add(attendance);

        //    await _context.SaveChangesAsync();
        //}
    }
}
