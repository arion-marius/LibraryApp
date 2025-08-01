using Application.Database.Books;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace Application.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBooksRepository _repository;

        public BooksController(IBooksRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _repository.GetBookByIdAsync(id);
            if (book == null)
                return NotFound();

            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookModel book)
        {
            if (!ModelState.IsValid)
            {
                return View(book);
            }

            await _repository.AddBookAsync(book);

            TempData["AlertMessage"] = $"The book \"{{book.Title}}\" has been added.";
            TempData["AlertType"] = "success";

            return RedirectToAction(nameof(GetBooksFromDb));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookModel book)
        {
            if (!ModelState.IsValid)
                return View(book); 

            await _repository.UpdateBookAsync(book);
            return RedirectToAction(nameof(GetBooksFromDb));
        }

        //[HttpGet]
        //public async Task<IActionResult> Index(int? page)
        //{
        //    var books = await _repository.GetBooksAsync();
        //    int pageSize = 5;
        //    int pageNumber = page ?? 1;

        //    var pagedBooks = books.ToPagedList(pageNumber, pageSize);
        //    ViewData["Action"] = "Index";
        //    return View(pagedBooks);
        //}

        [HttpGet]
        public async Task<IActionResult> GetBooksFromDb(string? search, int? page)
        {
            var books = await _repository.GetBooksFromDbAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                books = books
                    .Where(b => b.Title.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            int pageSize = 5;
            int pageNumber = page ?? 1;

            var pagedBooks = books.ToPagedList(pageNumber, pageSize);
            ViewData["Action"] = "GetBooksFromDb";
            ViewData["Search"] = search;
            return View("Index", pagedBooks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, message) = await _repository.DeleteBookAsync(id);

            TempData["AlertMessage"] = message;
            TempData["AlertType"] = success ? "success" : "warning";

            return RedirectToAction(nameof(GetBooksFromDb)); ;
        }
    }
}