using FollowUp.Data.Services;
using FollowUp.Data;
using FollowUp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace FollowUp.Areas.Trainer.Controllers
{
    [Authorize(Roles = StaticDetails.Trainer)]
    [Area(nameof(Trainer))]
    [Route(nameof(Trainer) + "/[controller]/[action]")]
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

            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _context.ApplicationUsers.FindAsync(userId);

            var Course = await _context.Tables
                .Include(x => x.ApplicationUser)
                .Include(y => y.Department)
                .Include(y => y.Build)
                .Include(y => y.Course)
                .Where(r => r.Activation.Status == "نشط" && r.ApplicationUser.UserName == user.UserName)
                .ToArrayAsync();


            return View(Course);
        }
    }
}
