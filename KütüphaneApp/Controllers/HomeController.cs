using Businnes.Abstract;
using KütüphaneApp.Models;
using KütüphaneApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KütüphaneApp.Controllers
{
    public class HomeController : Controller
    {
        private IBookService _bookService;
        private ICategoryService _categoryService;

        public HomeController(IBookService bookService, ICategoryService categoryService)
        {
            _bookService = bookService;
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            var bookViewModel = new BookViewModel()
            {
                Books = _bookService.GetAll()
            };
            return View(bookViewModel);
        }

        public IActionResult Search(string search)
        {
            var bookViewModel = new BookViewModel()
            {
                Categories = _categoryService.GetAll(),
                Books = _bookService.GetSearchResult(search),
            };
            return View(bookViewModel);
        }


        public IActionResult BookDetails(int id)
        {
            var model = _bookService.GetBookDetails(id);
            return View(model);
        }

        public IActionResult BookList(string category,int page = 1)
        {
            const int pageSize = 6;
            var bookViewModel = new BookViewModel()
            {
                PageInfo = new PageInfo()
                {
                    TotalItems = _bookService.GetCountByCategory(category),
                    CurrentPage = page,
                    ItemPerPage = pageSize,
                    CurrentCategory = category,
                },
                Categories = _categoryService.GetAll(),
                Books = _bookService.GetBooksByCategoryName(category,page,pageSize),
            };
            return View(bookViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}