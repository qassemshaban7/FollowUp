using FollowUp.Data.Services;
using FollowUp.Data;
using FollowUp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace FollowUp.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.Admin)]
    [Area(nameof(Admin))]
    [Route(nameof(Admin) + "/[controller]/[action]")]
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

            var Course = await _context.Attendances
                .Include(a => a.Table)
                    .ThenInclude(t => t.Department)
                    .Include(a => a.Table)
                    .ThenInclude(d => d.Build)
                    .Include(a => a.Table)
                    .ThenInclude(b => b.Course)
                .Include(a => a.ApplicationUser)
                .ToListAsync();

            return View(Course);
        }


        public async Task<IActionResult> SendOne(int id)
        {
            var Attendance = await _context.Attendances
                .Include(a => a.Table)
                    .ThenInclude(t => t.Department)
                .Include(a => a.Table)
                    .ThenInclude(d => d.Build)
                .Include(a => a.Table)
                    .ThenInclude(b => b.Course)
                .Include(a => a.ApplicationUser)
                .FirstOrDefaultAsync(r => r.Id == id);

            return View(Attendance);
        }

    }
}
