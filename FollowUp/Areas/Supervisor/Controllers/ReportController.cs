using FollowUp.Data.Services;
using FollowUp.Data;
using FollowUp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace FollowUp.Areas.Supervisor.Controllers
{
    [Authorize(Roles = StaticDetails.Supervisor)]
    [Area(nameof(Supervisor))]
    [Route(nameof(Supervisor) + "/[controller]/[action]")]
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
            var user = await _context.ApplicationUsers.Include(x => x.Department).FirstOrDefaultAsync(s => s.Id == userId);

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var Course = await _context.Attendances
                .Where(r => r.ApplicationUser.Department.Name == user.Department.Name)
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

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Attendances.FindAsync(id);

            _context.Attendances.Remove(user);
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("deleted", "true");
            return RedirectToAction(nameof(Index));
        }
    }
}
