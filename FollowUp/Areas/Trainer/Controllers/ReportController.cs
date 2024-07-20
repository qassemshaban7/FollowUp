using FollowUp.Data.Services;
using FollowUp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FollowUp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using FollowUp.Models;
using System.Security.Claims;

namespace FollowUp.Areas.Trainer.Controllers
{
    [Authorize(Roles = StaticDetails.Trainer)]
    [Area(nameof(Trainer))]
    [Route(nameof(Trainer) + "/[controller]/[action]")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailProvider _emailProvider;
        public ReportController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment
            , UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailProvider emailProvider)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailProvider = emailProvider;
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
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var Course = await _context.Attendances
                .Include(a => a.Table)
                    .ThenInclude(t => t.Department)
                    .Include(a => a.Table)
                    .ThenInclude(d => d.Build)
                    .Include(a => a.Table)
                    .ThenInclude(b => b.Course)
                .Include(a => a.ApplicationUser)
                .Where(c => c.ApplicationUser.Id == userId)
                .ToListAsync();


            return View(Course);
        }
    }
}
