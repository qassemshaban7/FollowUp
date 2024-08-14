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
        public async Task<IActionResult> Statistics(DateOnly? startDate, DateOnly? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                ViewBag.PresentStatistics = 0;
                ViewBag.LateStatistics = 0;
                ViewBag.AbsentStatistics = 0;
                ViewBag.TotalTables = 0;
                return View();
            }

            ViewData["SelectedStartDate"] = startDate.Value.ToString("yyyy-MM-dd");
            ViewData["SelectedEndDate"] = endDate.Value.ToString("yyyy-MM-dd");

            try
            {

                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var Super = await _context.ApplicationUsers.Include(x => x.Department).FirstOrDefaultAsync(v => v.Id == userId);

                DateTime startDateTime = startDate.Value.ToDateTime(new TimeOnly(0, 0));
                DateTime endDateTime = endDate.Value.ToDateTime(new TimeOnly(23, 59));

                var attendances = await _context.Attendances
                    .Where(x => x.Date >= startDateTime && x.Date <= endDateTime && x.ApplicationUser.Department.Name == Super.Department.Name)
                    .ToListAsync();

                var Absent = attendances.Count(x => x.Value == "غائب");
                var Late = attendances.Count(x => x.Value == "متأخر");

                var days = Enumerable.Range(0, (endDateTime - startDateTime).Days + 1)
                    .Select(offset => startDateTime.AddDays(offset))
                    .Select(date => DaysinArabic(date.DayOfWeek))
                    .ToList();

                var dayCounts = days.GroupBy(day => day)
                                    .ToDictionary(group => group.Key, group => group.Count());

                var tables = await _context.Tables
                    .Where(t => t.Activation.Status == "نشط" && t.ApplicationUser.Department.Name == Super.Department.Name)
                    .ToListAsync();

                var totalCount = 0;

                foreach (var day in dayCounts)
                {
                    var countForDay = tables.Count(t => t.Day == day.Key);
                    totalCount += countForDay * day.Value;
                }

                ViewBag.TotalTables = totalCount;
                ViewBag.PresentStatistics = totalCount - (Absent + Late);
                ViewBag.LateStatistics = Late;
                ViewBag.AbsentStatistics = Absent;
            }
            catch (ArgumentOutOfRangeException)
            {
                ViewBag.PresentStatistics = 0;
                ViewBag.LateStatistics = 0;
                ViewBag.AbsentStatistics = 0;
            }

            return View();
        }

        private string DaysinArabic(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Sunday => "الأحد",
                DayOfWeek.Monday => "الاثنين",
                DayOfWeek.Tuesday => "الثلاثاء",
                DayOfWeek.Wednesday => "الأربعاء",
                DayOfWeek.Thursday => "الخميس",
                DayOfWeek.Friday => "الجمعة",
                DayOfWeek.Saturday => "السبت",
                _ => throw new ArgumentOutOfRangeException()
            };
        }


        [HttpGet]
        public async Task<IActionResult> Absent(DateOnly? date, DateOnly? date2)
        {
            if (!date.HasValue)
            {
                return View(new List<Attendance>());
            }

            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var Super = await _context.ApplicationUsers.Include(x => x.Department).FirstOrDefaultAsync(v => v.Id == userId);

            DateTime startDateTime = date.Value.ToDateTime(new TimeOnly(0, 0));
            DateTime endDateTime = date2.Value.ToDateTime(new TimeOnly(23, 59));

            var Absent = _context.Attendances
                .Where(x => x.Date >= startDateTime && x.ApplicationUser.Department.Name == Super.Department.Name && x.Date <= endDateTime && x.Value == "غائب")
                .Include(a => a.Table)
                    .ThenInclude(t => t.Department)
                    .Include(a => a.Table)
                    .ThenInclude(d => d.Build)
                    .Include(a => a.Table)
                    .ThenInclude(b => b.Course)
                .Include(a => a.ApplicationUser)
                .ToList();

            return View(Absent);
        }

        [HttpGet]
        public async Task<IActionResult> Late(DateOnly? date, DateOnly? date2)
        {
            if (!date.HasValue)
            {
                return View(new List<Attendance>());
            }

            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var Super = await _context.ApplicationUsers.Include(x => x.Department).FirstOrDefaultAsync(v => v.Id == userId);

            DateTime startDateTime = date.Value.ToDateTime(new TimeOnly(0, 0));
            DateTime endDateTime = date2.Value.ToDateTime(new TimeOnly(23, 59));

            var Late = _context.Attendances
                .Where(x => x.Date >= startDateTime && x.ApplicationUser.Department.Name == Super.Department.Name && x.Date <= endDateTime && x.Value == "متأخر")
                .Include(a => a.Table)
                    .ThenInclude(t => t.Department)
                    .Include(a => a.Table)
                    .ThenInclude(d => d.Build)
                    .Include(a => a.Table)
                    .ThenInclude(b => b.Course)
                .Include(a => a.ApplicationUser)
                .ToList();

            return View(Late);
        }

    }
}
