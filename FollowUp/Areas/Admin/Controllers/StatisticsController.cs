using FollowUp.Data;
using FollowUp.Models;
using FollowUp.Utility;
using FollowUp.ViewModels;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public StatisticsController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment
            , UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
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

            var users = await (from x in _context.ApplicationUsers
                               join userRole in _context.UserRoles
                               on x.Id equals userRole.UserId
                               join role in _context.Roles
                               on userRole.RoleId equals role.Id
                               where role.Name == StaticDetails.Admin
                               && x.UserName != "Admin"
                               select x)
                               .Include(f => f.Department)
                               .ToListAsync();

            return View(users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(d => d.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                  .FirstOrDefaultAsync(d => d.Id == user.DepartmentId);
            if (department == null)
            {
                return NotFound();
            }

            var Roles = await _context.ApplicationRoles
                .Select(t => new SelectListItem
                {
                    Value = t.Name,
                    Text = t.ArabicRoleName
                })
                .ToListAsync();

            ViewBag.Roles = Roles;

            var selectedRoleNames = await _context.UserRoles
                .Where(userRole => userRole.UserId == id)
                .Join(_context.ApplicationRoles,
                    userRole => userRole.RoleId,
                    role => role.Id,
                    (userRole, role) => role.Name)
                .ToListAsync();

            var editUserVM = new EditSupervisorVM
            {
                Id = user.Id,
                FullName = user.UserFullName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DepartmentId = department.Id,
                SelectedRoles = selectedRoleNames,
            };

            ViewBag.Department = await _context.Departments.ToListAsync();
            return View(editUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditSupervisorVM editUserVM)
        {
            if (id != editUserVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _context.ApplicationUsers
                   .FirstOrDefaultAsync(d => d.Id == id);
                if (user == null)
                {
                    return NotFound();
                }

                user.UserFullName = editUserVM.FullName;
                user.UserName = editUserVM.UserName;
                user.Email = editUserVM.Email;
                user.PhoneNumber = editUserVM.PhoneNumber;
                user.DepartmentId = editUserVM.DepartmentId;

                string oldImageFileName = user.Image;
                string Oldd = user.Image;


                if (editUserVM.Image != null)
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");

                    string[] allowedExtensions = { ".png", ".jpg", ".jpeg" };
                    if (!allowedExtensions.Contains(Path.GetExtension(editUserVM.Image.FileName).ToLower()))
                    {
                        TempData["ErrorMessage"] = "Only .png and .jpg and .jpeg images are allowed!";
                        return RedirectToAction("Create");
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + editUserVM.Image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    await using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await editUserVM.Image.CopyToAsync(fileStream);
                    }

                    if (!string.IsNullOrEmpty(oldImageFileName))
                    {
                        string oldFilePath = Path.Combine(uploadsFolder, oldImageFileName);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    user.Image = uniqueFileName;
                }
                else
                {
                    user.Image = Oldd;
                }

                var selectedRoles = await _context.UserRoles
                    .Where(userRole => userRole.UserId == id)
                    .Select(userRole => userRole.RoleId)
                    .ToListAsync();

                var roleNames = await _context.ApplicationRoles
                    .Where(role => selectedRoles.Contains(role.Id))
                    .Select(role => role.Name)
                    .ToListAsync();


                await _userManager.RemoveFromRolesAsync(user, roleNames);
                await _context.SaveChangesAsync();

                await _userManager.AddToRolesAsync(user, editUserVM.SelectedRoles);
                await _context.SaveChangesAsync();

                if (editUserVM.NewPassword != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result2 = await _userManager.ResetPasswordAsync(user, token, editUserVM.NewPassword);
                    await _userManager.UpdateAsync(user);
                }

                _context.Update(user);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("updated", "true");
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Department = await _context.Departments.ToListAsync();
            return View(editUserVM);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.ApplicationUsers.FindAsync(id);

            _context.ApplicationUsers.Remove(user);
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("deleted", "true");
            return RedirectToAction(nameof(Index));
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
