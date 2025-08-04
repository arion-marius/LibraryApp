using Application.Database;
using Application.Database.Books;
using Application.Database.Readers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace Application.Controllers;

public class BooksController : Controller
{
    private readonly IBooksRepository _repository;
    private readonly IReadersRepository _readersRepository;

    public BooksController(IBooksRepository repository, IReadersRepository readersRepository)
    {
        _repository = repository;
        _readersRepository = readersRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetBooksFromDb(string? search, int? page)
    {
        var books = await _repository.GetBooksFromDbAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            books = books
                .Where(b => b.Title.Contains(search, System.StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        int pageSize = 5;
        int pageNumber = page ?? 1;

        var pagedBooks = books.ToPagedList(pageNumber, pageSize);
        ViewData["Action"] = "GetBooksFromDb";
        ViewData["Search"] = search;
        return View("Index", pagedBooks);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(BookModel book)
    {
        if (!ModelState.IsValid)
        {
            return View(book);
        }

        await _repository.AddBookAsync(book);

        TempData["AlertMessage"] = $"The book \"{book.Title}\" has been added.";
        TempData["AlertType"] = "success";

        return RedirectToAction(nameof(GetBooksFromDb));
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
    public async Task<IActionResult> Edit(BookModel book)
    {
        if (!ModelState.IsValid)
            return View(book);

        await _repository.UpdateBookAsync(book);
        TempData["AlertMessage"] = $"The book \"{book.Title}\" has been updated.";
        TempData["AlertType"] = "success";

        return RedirectToAction(nameof(GetBooksFromDb));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _repository.DeleteBookAsync(id);

        TempData["AlertMessage"] = message;
        TempData["AlertType"] = success ? "success" : "warning";

        return RedirectToAction(nameof(GetBooksFromDb));
    }
    [HttpPost]
    public async Task<IActionResult> ShowBorrowPopup(int bookId, string? cancel, int? page, string? search)
    {
        if (!string.IsNullOrEmpty(cancel))
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
            ViewData["ShowBorrowPopup"] = false;

            return View("Index", pagedBooks);
        }

        var readers = await _readersRepository.GetReadersFromDbAsync();
        ViewData["ReadersList"] = readers;
        ViewData["CurrentBookId"] = bookId;
        ViewData["ShowBorrowPopup"] = true;

        var booksList = await _repository.GetBooksFromDbAsync();
        var pagedBooksList = booksList.ToPagedList(1, 5);
        ViewData["Action"] = "GetBooksFromDb";
        ViewData["Search"] = "";

        return View("Index", pagedBooksList);
    }


    [HttpPost]
    public async Task<IActionResult> Borrow(int readerId, int bookId, string? cancel)
    {
        if (!string.IsNullOrEmpty(cancel))
        {
            return RedirectToAction(nameof(GetBooksFromDb));
        }

        var book = await _repository.GetBookByIdAsync(bookId);
        if (book == null || book.Stock <= 0)
        {
            TempData["AlertMessage"] = "Book is not available.";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetBooksFromDb));
        }

        if (await _readersRepository.HasReachedBorrowLimitAsync(readerId))
        {
            TempData["AlertMessage"] = "Reader has already borrowed 5 books.";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetBooksFromDb));
        }

        await _readersRepository.AddReaderBookAsync(readerId, bookId);

        book.Stock--;
        await _repository.UpdateBookAsync(book);

        TempData["AlertMessage"] = "Book borrowed successfully.";
        TempData["AlertType"] = "success";

        return RedirectToAction(nameof(GetBooksFromDb));

    }



}
