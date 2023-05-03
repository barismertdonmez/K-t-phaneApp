using Businnes.Abstract;
using Data.DataModels;
using Entity;
using KütüphaneApp.Identity;
using KütüphaneApp.Models;
using KütüphaneApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KütüphaneApp.Controllers
{
    [Authorize(Roles="Admin")]
    public class AdminController : Controller
    {
        
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;
        private IBookService _bookService;
        private ICategoryService _categoryService;
        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager,IBookService bookService,ICategoryService categoryService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _bookService = bookService;
            _categoryService = categoryService;
        }

        //*********************************
        // Identity Roles And Users Methods
        //*********************************
        public IActionResult RoleList()
        {
            return View(_roleManager.Roles);
        }

        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View();
        }

        public async Task<IActionResult> RoleEdit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            var members = new List<User>();
            var nonmembers = new List<User>();

            foreach (var user in _userManager.Users.ToList())
            {
                var list = await _userManager.IsInRoleAsync(user, role.Name)
                                ? members : nonmembers;
                list.Add(user);
            }
            var model = new RoleDetails()
            {
                Role = role,
                Members = members,
                NonMembers = nonmembers
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> RoleEdit(RoleEditModel model)
        {
            //if (ModelState.IsValid)
            //{
            foreach (var userId in model.IdsToAdd ?? new string[] { })
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var result = await _userManager.AddToRoleAsync(user, model.RoleName);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }

            foreach (var userId in model.IdsToDelete ?? new string[] { })
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            //}
            return Redirect("/admin/role/" + model.RoleId);
        }

        public IActionResult UserList()
        {
            return View(_userManager.Users);
        }

        public async Task<IActionResult> UserEdit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var selectedRoles = await _userManager.GetRolesAsync(user);
                var roles = _roleManager.Roles.Select(x => x.Name);
                ViewBag.Roles = roles;
                return View(new UserDetailsModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    SelectedRoles = selectedRoles,
                });
            }
            return Redirect("~/admin/user/list");
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserDetailsModel model, string[] selectedRoles)
        {
            //if (ModelState.IsValid)
            //{
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user != null)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.EmailConfirmed = model.EmailConfirmed;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    selectedRoles = selectedRoles ?? new string[] { };
                    await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles).ToArray<string>());
                    await _userManager.RemoveFromRolesAsync(user, selectedRoles.Except(userRoles).ToArray<string>());
                    return Redirect("/admin/user/list");
                }
            }
            //return Redirect("/admin/user/list");
            //}
            return View(model);
        }

        //********************************
        //********** Book Methods ********
        //********************************
        public IActionResult BookList()
        {
            return View(new BookViewModel()
            {
                Books = _bookService.GetAll(),
            }); 
        }

        public IActionResult CreateBook()
        {
            var categories = _categoryService.GetAll();
            ViewBag.Categories = categories;
            return View();
        }
        [HttpPost]
        public IActionResult CreateBook(DataBookCategoryModel model, int[] categories)
        {
            _bookService.CreateBookWithCategory(model.Book,categories.ToList());
            CreateMessage("Yeni Kayıt Eklendi","success");
            return RedirectToAction("BookList");
        }

        public IActionResult BookUpdate(int Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var entity = _bookService.GetByIdWithCategories(Id);
            if (entity == null)
            {
                return NotFound();
            }
            var model = new UpdateBookModel()
            {
                Id = entity.Id,
                BookName = entity.BookName,
                WriterName = entity.WriterName,
                ImageUrl = entity.ImageUrl,
                TotalPage = entity.TotalPage,
                SelectedCategories = entity.BookCategories.Select(c => c.Category).ToList(),
                Categories = _categoryService.GetAll(),
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult BookUpdate(UpdateBookModel model, int[] categoryIds)
        {
            var entity = _bookService.GetById(model.Id);
            if (entity == null)
            {
                return NotFound();
            }
            entity.BookName = model.BookName;
            entity.WriterName = model.WriterName;
            entity.ImageUrl = model.ImageUrl;
            entity.TotalPage = model.TotalPage;
            _bookService.Update(entity,categoryIds);
            return RedirectToAction("BookList");
        }

        public IActionResult AddNewCategoryOnBook(int id)
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddNewCategoryOnBook()
        {
            return View();
        }

        public IActionResult BookDelete(int bookId)
        {
            var entity = _bookService.GetById(bookId);
            if (entity != null)
            {
                _bookService.Delete(entity);
            };
            return RedirectToAction("BookList");
        }


        //*****************
        // Category Methods
        //*****************

        public IActionResult CategoryList()
        {
           return View(new BookViewModel()
           {
               Categories = _categoryService.GetAll()
           });
        }

        public IActionResult CategoryCreate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CategoryCreate(CategoryModel model)
        {
            var entity = new Category()
            {
                CategoryName = model.CategoryName,
            };
            _categoryService.Create(entity);
            return RedirectToAction("CategoryList");
        }

        public IActionResult CategoryEdit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var entity = _categoryService.GetByIdWithBooks(id);
            if (entity == null)
            {
                return NotFound();
            }
            var model = new CategoryModel()
            {
                Id = entity.Id,
                CategoryName = entity.CategoryName,
                Books = entity.BookCategories.Select(p => p.Book).ToList(),
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult CategoryEdit(CategoryModel model)
        {
            var entity = _categoryService.GetById(model.Id);
            if (entity == null)
            {
                return NotFound();
            }
            entity.CategoryName = model.CategoryName;
            _categoryService.Update(entity);
            return RedirectToAction("CategoryList");
        }

        public IActionResult DeleteCategory(int id)
        {
            var entity = _categoryService.GetById(id);
            if (entity != null)
            {
                _categoryService.Delete(entity);
            }
            return RedirectToAction("CategoryList");
        }

        public IActionResult DeleteFromCategory(int bookId,int categoryId)
        {
            _categoryService.DeleteFromCategory(bookId,categoryId);
            return RedirectToAction("CategoryList");
        }


        //****************
        //Yardımcı Methods
        //****************

        private void CreateMessage(string message, string alertType)
        {
            var msg = new AlertMessageModel()
            {
                Message = message.ToString(),
                AlertType = alertType,
            };
            TempData["message"] = JsonConvert.SerializeObject(msg);
        }

    }
}
