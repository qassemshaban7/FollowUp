using FollowUp.Data.Services;
using FollowUp.Data;
using FollowUp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FollowUp.Areas.HeadOfDept.Controllers
{
    [Authorize(Roles = StaticDetails.HeadOfDept)]
    [Area(nameof(HeadOfDept))]
    [Route(nameof(HeadOfDept) + "/[controller]/[action]")]
    public class PermissionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailProvider _emailProvider;
        public PermissionController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment
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
            var user = _context.ApplicationUsers.Include(x => x.Department).FirstOrDefault(y => y.Id == userId);

            var Course = await _context.Permissions
                .Where(c => c.ApplicationUser.Department.Name == user.Department.Name)
                .Include(x => x.ApplicationUser)
                .ToListAsync();

            return View(Course);
        }

        [HttpGet]
        public async Task<IActionResult> Report(int id)
        {
            var manager3 = await _context.configs.Where(x => x.Id == 3).FirstOrDefaultAsync();
            ViewBag.manager3 = manager3;

            var manager4 = await _context.configs.Where(x => x.Id == 4).FirstOrDefaultAsync();
            ViewBag.manager4 = manager4;

            var manager5 = await _context.configs.Where(x => x.Id == 5).FirstOrDefaultAsync();
            ViewBag.manager5 = manager5;

            var manager = await _context.configs.Where(x => x.Id == 1).FirstOrDefaultAsync();
            var manager2 = await _context.configs.Where(x => x.Id == 2).FirstOrDefaultAsync();
            ViewBag.manager = manager;
            ViewBag.manager2 = manager2;

            var repo = await _context.Permissions.Include(x => x.ApplicationUser).ThenInclude(v => v.Department).FirstOrDefaultAsync(c => c.Id == id);
            return View(repo);
        }

        public async Task<IActionResult> DeActivate(int? Id)
        {
            try
            {
                if (Id == null)
                    return NotFound();
                var service = await _context.Permissions.FirstOrDefaultAsync(m => m.Id == Id);
                if (service == null)
                    return NotFound();
                service.Status = 3;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }
        public async Task<IActionResult> Activate(int? Id)
        {
            try
            {
                if (Id == null)
                    return NotFound();
                var service = await _context.Permissions.FirstOrDefaultAsync(m => m.Id == Id);
                if (service == null)
                    return NotFound();
                service.Status = 2;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
