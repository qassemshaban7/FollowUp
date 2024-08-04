using FollowUp.Data;
using FollowUp.Models;
using FollowUp.Utility;
using FollowUp.ViewModels;
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
    public class SupervisorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public SupervisorController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment
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

            var users = await (from x in _context.ApplicationUsers
                               join userRole in _context.UserRoles
                               on x.Id equals userRole.UserId
                               join role in _context.Roles
                               on userRole.RoleId equals role.Id
                               where role.Name == StaticDetails.Supervisor
                               select x)
                               .Include(f => f.Department)
                               .ToListAsync();

            return View(users);
        }
        public async Task<IActionResult> Create(string? returnUrl = null)
        {
            ViewBag.Department = await _context.Departments.ToListAsync();
            ViewData["ReturnUrl"] = returnUrl;
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUser model, IFormFile? Image, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                ApplicationUser user = new()
                {
                    UserFullName = model.UserFullName,
                    UserName = model.UserName,
                    EmailConfirmed = true,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    DepartmentId = model.DepartmentId,
                };

                if (Image != null)
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");

                    string[] allowedExtensions = { ".png", ".jpg", ".jpeg" };
                    if (!allowedExtensions.Contains(Path.GetExtension(Image.FileName).ToLower()))
                    {
                        TempData["ErrorMessage"] = "Only .png and .jpg and .jpeg images are allowed!";
                        return RedirectToAction("Create");
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    await using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(fileStream);
                    }
                    user.Image = uniqueFileName;
                }

                var result = await _userManager.CreateAsync(user, "P@ssw0rd");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, StaticDetails.Supervisor);
                    await _context.SaveChangesAsync();

                    HttpContext.Session.SetString("created", "true");
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(d => d.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                  .FirstOrDefaultAsync(d => d.Id == user.DepartmentId);
            if (department == null)
            {
                return NotFound();
            }

            var Roles = await _context.ApplicationRoles
                .Select(t => new SelectListItem
                {
                    Value = t.Name,
                    Text = t.ArabicRoleName
                })
                .ToListAsync();

            ViewBag.Roles = Roles;

            var selectedRoleNames = await _context.UserRoles
                .Where(userRole => userRole.UserId == id)
                .Join(_context.ApplicationRoles,
                    userRole => userRole.RoleId,
                    role => role.Id,
                    (userRole, role) => role.Name)
                .ToListAsync();

            var editUserVM = new EditSupervisorVM
            {
                Id = user.Id,
                FullName = user.UserFullName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DepartmentId = department.Id,
                SelectedRoles = selectedRoleNames,
            };

            ViewBag.Department = await _context.Departments.ToListAsync();
            return View(editUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditSupervisorVM editUserVM)
        {
            if (id != editUserVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _context.ApplicationUsers
                   .FirstOrDefaultAsync(d => d.Id == id);
                if (user == null)
                {
                    return NotFound();
                }

                user.UserFullName = editUserVM.FullName;
                user.UserName = editUserVM.UserName;
                user.Email = editUserVM.Email;
                user.PhoneNumber = editUserVM.PhoneNumber;
                user.DepartmentId = editUserVM.DepartmentId;

                string oldImageFileName = user.Image;
                string Oldd = user.Image;


                if (editUserVM.Image != null)
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");

                    string[] allowedExtensions = { ".png", ".jpg", ".jpeg" };
                    if (!allowedExtensions.Contains(Path.GetExtension(editUserVM.Image.FileName).ToLower()))
                    {
                        TempData["ErrorMessage"] = "Only .png and .jpg and .jpeg images are allowed!";
                        return RedirectToAction("Create");
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + editUserVM.Image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    await using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await editUserVM.Image.CopyToAsync(fileStream);
                    }

                    if (!string.IsNullOrEmpty(oldImageFileName))
                    {
                        string oldFilePath = Path.Combine(uploadsFolder, oldImageFileName);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    user.Image = uniqueFileName;
                }
                else
                {
                    user.Image = Oldd;
                }

                var selectedRoles = await _context.UserRoles
                    .Where(userRole => userRole.UserId == id)
                    .Select(userRole => userRole.RoleId)
                    .ToListAsync();

                var roleNames = await _context.ApplicationRoles
                    .Where(role => selectedRoles.Contains(role.Id))
                    .Select(role => role.Name)
                    .ToListAsync();


                await _userManager.RemoveFromRolesAsync(user, roleNames);
                await _context.SaveChangesAsync();

                await _userManager.AddToRolesAsync(user, editUserVM.SelectedRoles);
                await _context.SaveChangesAsync();

                if (editUserVM.NewPassword != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result2 = await _userManager.ResetPasswordAsync(user, token, editUserVM.NewPassword);
                    await _userManager.UpdateAsync(user);
                }

                _context.Update(user);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("updated", "true");
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Department = await _context.Departments.ToListAsync();
            return View(editUserVM);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.ApplicationUsers.FindAsync(id);

            _context.ApplicationUsers.Remove(user);
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("deleted", "true");
            return RedirectToAction(nameof(Index));
        }

    }
}
