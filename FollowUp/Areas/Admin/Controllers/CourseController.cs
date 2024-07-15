using FollowUp.Data;
using FollowUp.Models;
using FollowUp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

namespace FollowUp.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.Admin)]
    [Area(nameof(Admin))]
    [Route(nameof(Admin) + "/[controller]/[action]")]
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public CourseController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment
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

            var Course = await _context.Courses.ToListAsync();

            return View(Course);
        }
        [HttpGet]
        public async Task<IActionResult> Create(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            try
            {

                Course course = new()
                {
                    Name = model.Name,
                    Coursecode = model.Coursecode,
                    ClassPart = model.ClassPart,
                    ReferenceNumber = model.ReferenceNumber,
                };

                _context.Courses.Add(course);

                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("created", "true");
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course editUserVM)
        {
            if (id != editUserVM.Id)
            {
                return NotFound();
            }

            try
            {
                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                {
                    return NotFound();
                }

                course.Name = editUserVM.Name;
                course.Coursecode = editUserVM.Coursecode;
                course.ClassPart = editUserVM.ClassPart;
                course.ReferenceNumber = editUserVM.ReferenceNumber;

                _context.Update(course);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("updated", "true");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(editUserVM);
            }
        }


        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("deleted", "true");
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }

    }
}
