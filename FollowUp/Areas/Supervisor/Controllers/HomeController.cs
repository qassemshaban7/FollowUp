using FollowUp.Data;
using FollowUp.Models;
using FollowUp.Utility;
using FollowUp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Data;
using System.Security.Claims;

namespace FollowUp.Areas.Supervisor.Controllers
{
    [Authorize(Roles = StaticDetails.Supervisor)]
    [Area(nameof(Supervisor))]
    [Route(nameof(Supervisor) + "/[controller]/[action]")]
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
            var user = await _context.ApplicationUsers.Include(x => x.Department).FirstOrDefaultAsync( v => v.Id == userId);

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }


            var Tables = await _context.Tables.Where(r => r.Activation.Status == "نشط" && (r.TypeDivition == "نظري صباحي" || r.TypeDivition == "عملي صباحي") && r.ApplicationUser.Department.Name == user.Department.Name).CountAsync();
            ViewBag.Tables = Tables;

            var Evening = await _context.Tables.Where(r => r.Activation.Status == "نشط" && (r.TypeDivition == "نظري مسائي" || r.TypeDivition == "عملي مسائي") && r.ApplicationUser.Department.Name == user.Department.Name).CountAsync();
            ViewBag.Evening = Evening;

            var Report = await _context.Attendances.Where(r => r.ApplicationUser.Department.Name == user.Department.Name).CountAsync();
            ViewBag.Report = Report;

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
            if (newPassword == null)
            {
                x = 2;
                return View("ChangePassword", new ChangePasswordViewModel { X = x });
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
