using Businnes.Abstract;
using Data.Abstract;
using KütüphaneApp.Identity;
using KütüphaneApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KütüphaneApp.Controllers
{
    public class AccountController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private ICartService _cartService;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ICartService cartService, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _cartService = cartService;
            _roleManager = roleManager;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user =await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Bu kullanıcıc adı ile daha önce bir hesap açılmamaış");
                return View(model);
            }
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Bu mail adresi onaylı değil,lütfen hesabınızı onaylayın");
                return View(model);
            }
            var result = await _signInManager.PasswordSignInAsync(user.UserName,model.Password,false,false);
            if (result.Succeeded)
            {
                return Redirect("~/");
            }
            ModelState.AddModelError("", "Girilen kullanıcı maili veya parolası hatalı");
            return View(model);
        }

        
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email,
            };
            
            var result = await _userManager.CreateAsync(user,model.Password);
            if (result.Succeeded)
            {
                _cartService.InitializeCart(user.Id);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = user.Id,
                    token = code,
                });
                Console.WriteLine(url);
                return RedirectToAction("Login","Account");
            }
            
            ModelState.AddModelError("", "Geçersiz işlem lütfen tekrar deneyin");
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }


        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                CreateMessage("Geçersiz Token Bilgisi","danger");
                return View();
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    CreateMessage("Hesabınız Onaylandı","success");
                    return View();
                }
            }
            CreateMessage("Hesabınız Onaylanmadı","warning");
            return View();
        }

        public async Task<IActionResult> UserUpdate()
        {
            var id = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var model = new UserUpdateModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UserUpdate(UserUpdateModel model)
        {
            var id = _userManager.GetUserId(User);
            var user =await _userManager.FindByIdAsync(id);
            user.Id = model.Id;
            user.UserName = model.UserName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;

            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index", "Home");

            
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
