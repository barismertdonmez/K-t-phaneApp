using Businnes.Abstract;
using KütüphaneApp.Identity;
using KütüphaneApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KütüphaneApp.Controllers
{
    public class CartController : Controller
    {
        private ICartService _cartService;
        private UserManager<User> _userManager;
        public CartController(ICartService cartService, UserManager<User> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }
        [Authorize]
        public IActionResult Index()
        {
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));
            return View(new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(c => new CartItemModel()
                {
                    CartItemId = c.Id,
                    BookName = c.Book.BookName,
                    BookId = c.Book.Id,
                    ImageUrl = c.Book.ImageUrl,
                    Quantity = c.Quantity,
                }).ToList(),
            });
        }

        [HttpPost]
        public IActionResult AddToCart(int bookId,int quantity)
        {
            var userId = _userManager.GetUserId(User);
            _cartService.AddToCart(userId, bookId, quantity);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteFromCart(int bookId)
        {
            var userId = _userManager.GetUserId(User);
            _cartService.DeleteFromCart(userId, bookId);
            return RedirectToAction("Index");
        }
    }
}
