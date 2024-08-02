using FollowUp.Data;
using FollowUp.Models;
using FollowUp.Utility;
using FollowUp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Security.Claims;

namespace FollowUp.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.Admin)]
    [Area(nameof(Admin))]
    [Route(nameof(Admin) + "/[controller]/[action]")]
    public class TrainerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public TrainerController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment
            , UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add()
        {
            try
            {
                var files = HttpContext.Request.Form.Files;

                if (files.Count > 0)
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var uploads = Path.Combine(webRootPath, @"TrainerFiles\");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + files[0].FileName;

                    using (var filesStream = new FileStream(Path.Combine(uploads, uniqueFileName), FileMode.Create))
                    {
                        files[0].CopyTo(filesStream);
                    }

                    using (var package = new ExcelPackage(new FileInfo(Path.Combine(uploads, uniqueFileName))))
                    {
                        var worksheet = package.Workbook.Worksheets[0];

                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            string deptName = worksheet.Cells[row, 8].Value.ToString();
                            if (deptName == null) continue;
                            var department = await _context.Departments.FirstOrDefaultAsync(x => x.Name == deptName);
                            if (department == null)
                            {
                                department = new Department
                                {
                                    Name = deptName
                                };
                                _context.Departments.Add(department);
                                await _context.SaveChangesAsync();
                            }
                            var user = new ApplicationUser
                            {
                                UserFullName = worksheet.Cells[row, 3].Value.ToString(),
                                UserName = worksheet.Cells[row, 2].Value.ToString(),
                                Specialty = worksheet.Cells[row, 7].Value?.ToString() ?? null,
                                Email = worksheet.Cells[row, 9].Value?.ToString() ?? null,
                                PhoneNumber = worksheet.Cells[row, 10].Value?.ToString() ?? null,
                                DepartmentId = department.Id,
                                EmailConfirmed = true,
                            };

                            string sp = worksheet.Cells[row, 11].Value?.ToString();

                            if (!string.IsNullOrEmpty(sp) && sp != "رئيس قسم" && sp != "العميد")
                            {
                                continue;
                            }

                            var result = await _userManager.CreateAsync(user, "P@ssw0rd");

                            if (result.Succeeded)
                            {
                                if (sp == "رئيس قسم")
                                {
                                    await _userManager.AddToRoleAsync(user, StaticDetails.HeadOfDept);
                                    await _context.SaveChangesAsync();
                                }
                                else if (sp == "العميد")
                                {
                                    await _userManager.AddToRoleAsync(user, StaticDetails.Admin);
                                    await _context.SaveChangesAsync();
                                }
                                else if (string.IsNullOrEmpty(sp))
                                {
                                    await _userManager.AddToRoleAsync(user, StaticDetails.Trainer);
                                    await _context.SaveChangesAsync();
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    HttpContext.Session.SetString("created", "true");
                }
                return RedirectToAction(nameof(Index));

            }

            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
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
                               where role.Name == StaticDetails.Trainer
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
                    DepartmentId = model.DepartmentId,
                    Email = model.Email,
                    EmailConfirmed = true,
                    //PhoneNumber = model.PhoneNumber,
                    //Specialty = model.Specialty,
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
                    await _userManager.AddToRoleAsync(user, StaticDetails.Trainer);
                    await _context.SaveChangesAsync();

                    HttpContext.Session.SetString("created", "true");
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ViewBag.Department = await _context.Departments.ToListAsync();
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
                .Include(x => x.Department)
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

            var editUserVM = new EditTrainerVM
            {
                Id = user.Id,
                FullName = user.UserFullName,
                UserName = user.UserName,
                DepartmentId = department.Id,
                Email = user.Email,
                //PhoneNumber = user.PhoneNumber,
                //Specialty = user.Specialty,
            };


            ViewBag.Department = await _context.Departments.ToListAsync();
            return View(editUserVM);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditTrainerVM editUserVM)
        {
            if (id != editUserVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _context.ApplicationUsers
                   .Include(x => x.Department)
                   .FirstOrDefaultAsync(d => d.Id == id);
                if (user == null)
                {
                    return NotFound();
                }

                user.UserFullName = editUserVM.FullName;
                user.UserName = editUserVM.UserName;
                user.Email = editUserVM.Email;
                user.DepartmentId = editUserVM.DepartmentId;
                //user.PhoneNumber = editUserVM.PhoneNumber;
                //user.Specialty = editUserVM.Specialty;

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

            //var tabels = await _context.Tables.FirstOrDefaultAsync(x => x.TrainerId == id);
            //_context.Tables.RemoveRange(tabels);
            //await _context.SaveChangesAsync();

            var user = await _context.ApplicationUsers.FindAsync(id);

            _context.ApplicationUsers.Remove(user);
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("deleted", "true");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Report(string id)
        {
            var user = await _context.ApplicationUsers.FindAsync(id);

            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var admin = await _context.ApplicationUsers.Include(x => x.Department).FirstOrDefaultAsync(v => v.Id == userId);

            ViewBag.Admin = admin;

            var Atten = await _context.Attendances.Where(c => c.ApplicationUser.Id == id && c.Value == "غائب").CountAsync();
            ViewBag.Atten = Atten;

            var Late = await _context.Attendances.Where(c => c.ApplicationUser.Id == id && c.Value == "متأخر").CountAsync();
            ViewBag.Late = Late;

            var Minutes = await _context.Attendances.Where(c => c.ApplicationUser.Id == id).SumAsync(d => d.Minutes);
            ViewBag.Minutes = Minutes;

            return View(user);
        }

        [HttpGet("DownloadExcel")]
        public async Task<IActionResult> DownloadExcel()
        {
            try
            {
                string FileName = "جدول منسوبي الكلية الجديد.xlsx";
                string exampleFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Example");
                var filePath = Path.Combine(exampleFolder, FileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound();
                }

                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(filePath, out var contentType))
                {
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

                return File(fileBytes, contentType, Path.GetFileName(filePath));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
