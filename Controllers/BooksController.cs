using Application.Database;
using Application.Database.Books;
using Application.Database.CustomExceptions;
using Application.Database.Readers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace Application.Controllers;

public class BooksController : Controller
{
    private readonly IBooksRepository _bookRepository;
    private readonly IReadersRepository _readersRepository;

    public BooksController(IBooksRepository repository, IReadersRepository readersRepository)
    {
        _bookRepository = repository;
        _readersRepository = readersRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetBooksFromDb(string? search, int? page)
    {
        var books = await _bookRepository.GetBooksAsync(search);

        if (!string.IsNullOrWhiteSpace(search))
        {
            books = books
                .Where(b => b.Title.Contains(search, System.StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        int pageSize = 5;
        int pageNumber = page ?? 1;

        //var pagedBooks = books.ToPagedList(pageNumber, pageSize);
        //var pagedBooks = books.ToPagedList(pageNumber, pageSize);
        ViewData["Action"] = nameof(GetBooksFromDb);
        ViewData["Search"] = search;
        books.Add(new() { Id = 1000, Title = "ce vreau eu", Stock = 3 });

        return View("Index", books.ToPagedList(pageNumber, pageSize));
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

        await _bookRepository.AddBookAsync(book);

        TempData["AlertMessage"] = $"The book {book.Title} has been added.";
        TempData["AlertType"] = "success";

        return RedirectToAction(nameof(GetBooksFromDb));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _bookRepository.GetBookByIdAsync(id);
        return View(book);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(BookModel book)
    {
        if (!ModelState.IsValid)
            return View(book);

        await _bookRepository.UpdateBookAsync(book);
        TempData["AlertMessage"] = $"The book \"{book.Title}\" has been updated.";
        TempData["AlertType"] = "success";

        return RedirectToAction(nameof(GetBooksFromDb));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _bookRepository.DeleteBookAsync(id);

        TempData["AlertMessage"] = message;
        TempData["AlertType"] = success ? "success" : "warning";

        return RedirectToAction(nameof(GetBooksFromDb));
    }

    [HttpPost]
    public async Task<IActionResult> ShowBorrowPopup(int bookId, string search = "")
    {
        var readers = await _readersRepository.GetPaginatedReadersFromDbAsync();
        if (!string.IsNullOrWhiteSpace(search))
        {
            readers = readers
                .Where(r => r.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        ViewData["ReadersList"] = readers;
        ViewData["CurrentBookId"] = bookId;
        ViewData["ShowBorrowPopup"] = true;
        ViewData["SearchTerm"] = search;

        var booksList = await _bookRepository.GetBooksAsync(search);
        var pagedBooksList = booksList.ToPagedList(1, 5);
        ViewData["Action"] = "GetBooksFromDb";
        ViewData["Search"] = "";

        return View("Index", pagedBooksList);
    }

    [HttpPost]
    public async Task<IActionResult> Borrow(int readerId, int bookId)
    {
        try
        {
            await _bookRepository.BorrowAsync(bookId, readerId);
        }
        catch (BookNotFoundException)
        {
            TempData["AlertMessage"] = "Book is not available.";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetBooksFromDb));
        }
        catch (ReaderNotFoundException)
        {
            TempData["AlertMessage"] = "Reader is not available.";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetBooksFromDb));
        }
        catch (TooManyBooksException)
        {
            TempData["AlertMessage"] = "Reader has already borrowed 5 books.";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetBooksFromDb));
        }
        catch (BookOutOfStockException)
        {
            TempData["AlertMessage"] = "Book is out of stock";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetBooksFromDb));
        }
        catch (BookAlreadyBorrowedException)
        {
            TempData["AlertMessage"] = "The book is already borrowed.";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetBooksFromDb));
        }

        TempData["AlertMessage"] = "Book borrowed successfully.";
        TempData["AlertType"] = "success";

        return RedirectToAction(nameof(GetBooksFromDb));
    }
}