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

namespace FollowUp.Areas.HeadOfDept.Controllers
{
    [Authorize(Roles = StaticDetails.HeadOfDept)]
    [Area(nameof(HeadOfDept))]
    [Route(nameof(HeadOfDept) + "/[controller]/[action]")]
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
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var Tra = await _context.ApplicationUsers.Include(x => x.Department).FirstOrDefaultAsync(v => v.Id == userId);

            var Supervisors = await (from user in _context.ApplicationUsers
                                  join userRole in _context.UserRoles
                                  on user.Id equals userRole.UserId
                                  join role in _context.Roles
                                  on userRole.RoleId equals role.Id
                                  where role.Name == StaticDetails.Supervisor
                                  select user)
                                  .Where(w => w.Department.Name == Tra.Department.Name).CountAsync();

            ViewBag.Supervisors = Supervisors;

            var trainers = await (from user in _context.ApplicationUsers
                                  join userRole in _context.UserRoles
                                  on user.Id equals userRole.UserId
                                  join role in _context.Roles
                                  on userRole.RoleId equals role.Id
                                  where role.Name == StaticDetails.Trainer
                                  select user)
                                  .Where(w => w.Department.Name == Tra.Department.Name).CountAsync();

            ViewBag.Trainers = trainers;

            var Tables = await _context.Tables
                .Where(r => r.Activation.Status == "نشط" && r.ApplicationUser.Department.Name == Tra.Department.Name)
                .CountAsync();

            ViewBag.Tables = Tables;

            var Report = await _context.Attendances
                .Where(x => x.ApplicationUser.Department.Name == Tra.Department.Name).CountAsync();
            ViewBag.Report = Report;

            var Permissions = await (from permission in _context.Permissions
                                     join user in _context.ApplicationUsers
                                     on permission.TrainerId equals user.Id
                                     join userRole in _context.UserRoles
                                     on user.Id equals userRole.UserId
                                     join role in _context.Roles
                                     on userRole.RoleId equals role.Id
                                     where role.Name == StaticDetails.Trainer &&
                                           user.Department.Name == Tra.Department.Name
                                     select permission)
                                    .CountAsync();

            ViewBag.Permissions = Permissions;

            return View();
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
