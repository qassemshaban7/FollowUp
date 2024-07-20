using FollowUp.Data;
using FollowUp.Models;
using FollowUp.Utility;
using FollowUp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations.Schema;

namespace FollowUp.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.Admin)]
    [Area(nameof(Admin))]
    [Route(nameof(Admin) + "/[controller]/[action]")]
    public class TableController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public TableController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment
            , UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int AcId)
        {
            try
            {
                var files = HttpContext.Request.Form.Files;

                if (files.Count > 0)
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var uploads = Path.Combine(webRootPath, @"TabelsFiles\");
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
                            string deptName = worksheet.Cells[row, 4].Value?.ToString();
                            if (string.IsNullOrEmpty(deptName))
                                continue;

                            var department = await _context.Departments.FirstOrDefaultAsync(x => x.Name == deptName);
                            if (department == null)
                            {
                                department = new Department { Name = deptName };
                                _context.Departments.Add(department);
                                await _context.SaveChangesAsync();
                            }

                            string coursecode = worksheet.Cells[row, 5].Value?.ToString();
                            if (string.IsNullOrEmpty(coursecode))
                                continue;

                            var course = await _context.Courses.FirstOrDefaultAsync(x => x.Coursecode == coursecode);

                            if (course == null)
                            {
                                course = new Course
                                {
                                    Name = worksheet.Cells[row, 6].Value?.ToString(),
                                    ClassPart = worksheet.Cells[row, 3].Value?.ToString(),
                                    Coursecode = coursecode,
                                    ReferenceNumber = Convert.ToInt32(worksheet.Cells[row, 7].Value)
                                };

                                await _context.Courses.AddAsync(course);
                                await _context.SaveChangesAsync();
                            }

                            string username = worksheet.Cells[row, 19].Value?.ToString();

                            string email = worksheet.Cells[row, 21].Value?.ToString();

                            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.UserName == username);
                            if (user == null)
                            {
                                if (email == null)
                                {
                                    user = new ApplicationUser
                                    {
                                        UserFullName = worksheet.Cells[row, 20].Value?.ToString(),
                                        UserName = username,
                                        DepartmentId = department.Id,
                                        EmailConfirmed = true,
                                    };

                                    var result = await _userManager.CreateAsync(user, "P@ssw0rd");

                                    if (result.Succeeded)
                                    {
                                        await _userManager.AddToRoleAsync(user, StaticDetails.Trainer);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    user = new ApplicationUser
                                    {
                                        UserFullName = worksheet.Cells[row, 20].Value?.ToString(),
                                        UserName = username,
                                        DepartmentId = department.Id,
                                        EmailConfirmed = true,
                                        Email = email,
                                    };

                                    var result = await _userManager.CreateAsync(user, "P@ssw0rd");

                                    if (result.Succeeded)
                                    {
                                        await _userManager.AddToRoleAsync(user, StaticDetails.Trainer);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }

                            string hall = worksheet.Cells[row, 15].Value?.ToString();
                            if (string.IsNullOrEmpty(hall))
                                continue;

                            var build = await _context.Builds.FirstOrDefaultAsync(x => x.Hall == hall);

                            if (build == null)
                            {
                                build = new Build
                                {
                                    Hall = hall,
                                    Building = worksheet.Cells[row, 14].Value?.ToString(),
                                    TrainingUnit = worksheet.Cells[row, 2].Value?.ToString(),
                                    Classroom = worksheet.Cells[row, 1].Value?.ToString(),
                                };

                                await _context.Builds.AddAsync(build);
                                await _context.SaveChangesAsync();
                            }

                            var table = new Table
                            {
                                ContactHours = Convert.ToInt32(worksheet.Cells[row, 9].Value),
                                AccountingHours = Convert.ToInt32(worksheet.Cells[row, 8].Value),
                                TypeDivition = worksheet.Cells[row, 10].Value?.ToString(),
                                Day = worksheet.Cells[row, 11].Value?.ToString(),
                                Time = worksheet.Cells[row, 12].Value?.ToString(),
                                Capacity = Convert.ToInt32(worksheet.Cells[row, 16].Value),
                                Registered = Convert.ToInt32(worksheet.Cells[row, 17].Value),
                                Stay = Convert.ToInt32(worksheet.Cells[row, 18].Value),
                                DepartmentId = department.Id,
                                CourseId = course.Id,
                                BuildId = build.Id,
                                ActivationId = AcId,
                                TrainerId = user.Id,
                                ApplicationUser = user,
                            };

                            await _context.Tables.AddAsync(table);
                            await _context.SaveChangesAsync();
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

            var Course = await _context.Tables
                .Include(x => x.ApplicationUser)
                .Include(y => y.Department)
                .Include(y => y.Activation)
                .Include(y => y.Build)
                .Include(y => y.Course)
                .Where(r => r.Activation.Status == "نشط")
                .ToListAsync();

            ViewBag.Actvations = await _context.Activations.ToListAsync();

            return View(Course);
        }
        [HttpGet]
        public async Task<IActionResult> Create(string? returnUrl = null)
        {
            var users = await (from x in _context.ApplicationUsers
                               join userRole in _context.UserRoles
                               on x.Id equals userRole.UserId
                               join role in _context.Roles
                               on userRole.RoleId equals role.Id
                               where role.Name == StaticDetails.Trainer
                               select new SelectListItem
                               {
                                   Value = x.Id.ToString(),
                                   Text = x.UserFullName
                               }).ToListAsync();

            ViewBag.Trainers = users;

            ViewBag.Activation = await _context.Activations.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToListAsync();

            ViewBag.Department = await _context.Departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            }).ToListAsync();

            ViewBag.Course = await _context.Courses.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToListAsync();

            ViewBag.Build = await _context.Builds.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Hall
            }).ToListAsync();

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Table model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            try
            {
                var user = await _context.ApplicationUsers.FindAsync(model.TrainerId);

                Table table = new()
                {
                    ContactHours = model.ContactHours,
                    AccountingHours = model.AccountingHours,
                    TypeDivition = model.TypeDivition,
                    Day = model.Day,
                    Time = model.Time,
                    Capacity = model.Capacity,
                    Registered = model.Registered,
                    TrainerId = model.TrainerId,
                    DepartmentId = model.DepartmentId,
                    CourseId = model.CourseId,
                    BuildId = model.BuildId,
                    Stay = model.Stay,
                    ActivationId = model.ActivationId,
                    ApplicationUser = user,
                };

                _context.Tables.Add(table);

                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("created", "true");
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                var users = await (from x in _context.ApplicationUsers
                                   join userRole in _context.UserRoles
                                   on x.Id equals userRole.UserId
                                   join role in _context.Roles
                                   on userRole.RoleId equals role.Id
                                   where role.Name == StaticDetails.Trainer
                                   select new SelectListItem
                                   {
                                       Value = x.Id.ToString(),
                                       Text = x.UserFullName
                                   }).ToListAsync();

                ViewBag.Trainers = users;

                ViewBag.Activation = await _context.Activations.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToListAsync();

                ViewBag.Department = await _context.Departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                }).ToListAsync();

                ViewBag.Course = await _context.Courses.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToListAsync();

                ViewBag.Build = await _context.Builds.Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Hall
                }).ToListAsync();

                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Tables.FindAsync(id);
            if (table == null)
            {
                return NotFound();
            }

            var editUserVM = new EditTableVM
            {
                ContactHours = table.ContactHours,
                AccountingHours = table.AccountingHours,
                TypeDivition = table.TypeDivition,
                Day = table.Day,
                Time = table.Time,
                Capacity = table.Capacity,
                Registered = table.Registered,
                Stay = table.Stay,
                DepartmentId = table.DepartmentId,
                CourseId = table.CourseId,
                TrainerId = table.TrainerId,
                BuildId = table.BuildId,
                ActivationId = table.ActivationId,
            };

            var users = await (from x in _context.ApplicationUsers
                               join userRole in _context.UserRoles
                               on x.Id equals userRole.UserId
                               join role in _context.Roles
                               on userRole.RoleId equals role.Id
                               where role.Name == StaticDetails.Trainer
                               select new SelectListItem
                               {
                                   Value = x.Id.ToString(),
                                   Text = x.UserFullName
                               }).ToListAsync();

            ViewBag.Trainers = users;

            ViewBag.Activation = await _context.Activations.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToListAsync();

            ViewBag.Department = await _context.Departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            }).ToListAsync();

            ViewBag.Course = await _context.Courses.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToListAsync();

            ViewBag.Build = await _context.Builds.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Hall
            }).ToListAsync();

            return View(editUserVM);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditTableVM editUserVM)
        {
            if (id != editUserVM.Id)
            {
                return NotFound();
            }

            try
            {
                var table = await _context.Tables.FindAsync(id);
                if (table == null)
                {
                    return NotFound();
                }

                var user1 = await _context.ApplicationUsers.FindAsync(table.TrainerId);

                table.ContactHours = editUserVM.ContactHours;
                table.AccountingHours = editUserVM.AccountingHours;
                table.TypeDivition = editUserVM.TypeDivition;
                table.Day = editUserVM.Day;
                table.Time = editUserVM.Time;
                table.Capacity = editUserVM.Capacity;
                table.Registered = editUserVM.Registered;
                table.Stay = editUserVM.Stay;
                table.DepartmentId = editUserVM.DepartmentId;
                table.CourseId = editUserVM.CourseId;
                table.TrainerId = editUserVM.TrainerId;
                table.BuildId = editUserVM.BuildId;
                table.ActivationId = editUserVM.ActivationId;
                table.ApplicationUser = user1;

                _context.Update(table);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("updated", "true");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                var users = await (from x in _context.ApplicationUsers
                                   join userRole in _context.UserRoles
                                   on x.Id equals userRole.UserId
                                   join role in _context.Roles
                                   on userRole.RoleId equals role.Id
                                   where role.Name == StaticDetails.Trainer
                                   select new SelectListItem
                                   {
                                       Value = x.Id.ToString(),
                                       Text = x.UserFullName
                                   }).ToListAsync();

                ViewBag.Trainers = users;

                ViewBag.Activation = await _context.Activations.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToListAsync();

                ViewBag.Department = await _context.Departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                }).ToListAsync();

                ViewBag.Course = await _context.Courses.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToListAsync();

                ViewBag.Build = await _context.Builds.Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Hall
                }).ToListAsync();

                return View(editUserVM);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Tables.FindAsync(id);

            _context.Tables.Remove(course);
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("deleted", "true");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("DownloadExcel")]
        public async Task<IActionResult> DownloadExcel()
        {
            try
            {
                string FileName = "مثال اضافة الجداول.xlsx";
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
