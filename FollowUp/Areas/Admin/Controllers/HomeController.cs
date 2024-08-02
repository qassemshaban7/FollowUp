using FollowUp.Data;
using FollowUp.Models;
using FollowUp.Utility;
using FollowUp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace FollowUp.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.Admin)]
    [Area(nameof(Admin))]
    [Route(nameof(Admin) + "/[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var Supervisors = await (from user in _context.ApplicationUsers
                                  join userRole in _context.UserRoles
                                  on user.Id equals userRole.UserId
                                  join role in _context.Roles
                                  on userRole.RoleId equals role.Id
                                  where role.Name == StaticDetails.Supervisor
                                  select user).CountAsync();

            ViewBag.Supervisors = Supervisors;

            var trainers = await (from user in _context.ApplicationUsers
                                  join userRole in _context.UserRoles
                                  on user.Id equals userRole.UserId
                                  join role in _context.Roles
                                  on userRole.RoleId equals role.Id
                                  where role.Name == StaticDetails.Trainer
                                  select user).CountAsync();

            ViewBag.Trainers = trainers;

            var HeadOfDept = await (from user in _context.ApplicationUsers
                                  join userRole in _context.UserRoles
                                  on user.Id equals userRole.UserId
                                  join role in _context.Roles
                                  on userRole.RoleId equals role.Id
                                  where role.Name == StaticDetails.HeadOfDept
                                  select user).CountAsync();

            ViewBag.HeadOfDept = HeadOfDept;

            var Courses = await _context.Courses.CountAsync();
            ViewBag.Course = Courses;

            var Building = await _context.Builds.CountAsync();
            ViewBag.Building = Building;

            var Tables = await _context.Tables.Where(r => r.Activation.Status == "نشط").CountAsync();
            ViewBag.Tables = Tables;

            var Departments = await _context.Departments.CountAsync();
            ViewBag.Departments = Departments;

            var Report = await _context.Attendances.CountAsync();
            ViewBag.Report = Report;

            var Activation = await _context.Activations.CountAsync();
            ViewBag.Activation = Activation;

            var Permissions = await _context.Permissions.CountAsync();
            ViewBag.Permissions = Permissions;

            SuperAdminHomeVM homeVM = new SuperAdminHomeVM
            {
                
            };

            return View(homeVM);
        }

        public async Task<IActionResult> ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Password(string oldPassword, string newPassword)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);

            int x = 0;
            if (oldPassword == null && newPassword == null)
            {
                {
                    x = 2;
                    return View("ChangePassword", new ChangePasswordViewModel { X = x });
                }
            }

            var passwordVerificationResult = await _userManager.CheckPasswordAsync(user, oldPassword);
            if (!passwordVerificationResult)
            {
                x = 1;
                return View("ChangePassword", new ChangePasswordViewModel { X = x });
            }

            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            else
            {
                return View("ChangePassword", new ChangePasswordViewModel());
            }
        }
    }
}
