using FollowUp.Data;
using FollowUp.Models;
using FollowUp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FollowUp.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.Admin)]
    [Area(nameof(Admin))]
    [Route(nameof(Admin) + "/[controller]/[action]")]
    public class BuildController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public BuildController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment
            , UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
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

            var surveys = await _context.Builds.ToListAsync();

            return View(surveys);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Build model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            try
            {
                var survey = new Build
                {
                    Hall = model.Hall,
                    Building = model.Building,
                    Classroom = model.Classroom,
                    TrainingUnit = model.TrainingUnit,
                };

                _context.Builds.Add(survey);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("created", "true");
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var survey = await _context.Builds.FirstOrDefaultAsync(s => s.Id == id);
            if (survey == null)
            {
                return NotFound();
            }
            return View(survey);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Build model)
        {
            try
            {
                var survey = await _context.Builds.FirstOrDefaultAsync(s => s.Id == model.Id);

                if (survey == null)
                {
                    return NotFound();
                }

                survey.Hall = model.Hall;
                survey.TrainingUnit = model.TrainingUnit;
                survey.Classroom = model.Classroom;
                survey.Building = model.Building;

                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("updated", "true");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }


        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Builds.FindAsync(id);

            _context.Builds.Remove(course);
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("deleted", "true");
            return RedirectToAction(nameof(Index));
        }
    }
}
