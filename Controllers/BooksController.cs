using Application.Database.Books;
using Application.Database.CustomExceptions;
using Application.Database.Readers;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using X.PagedList;

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
    public IActionResult GetBooksFromDb(string? search, int page = 1)
    {
        var pagedBooks = _bookRepository.GetPagedBooks(search, page);

        ViewData["Action"] = nameof(GetBooksFromDb);
        ViewData["Search"] = search;

        var serializablePagedList = new SerializablePagedList<BookDto>
        {
            Items = pagedBooks.ToList(),
            PageNumber = pagedBooks.PageNumber,
            PageSize = pagedBooks.PageSize,
            TotalItemCount = pagedBooks.TotalItemCount,
        };
        TempData["Books"] = JsonSerializer.Serialize(serializablePagedList);
        TempData.Keep();
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
        var readers = await _readersRepository.GetPaginatedReadersFromDbAsync(search);

        var deserialized = JsonSerializer.Deserialize<SerializablePagedList<BookDto>>(TempData["Books"] as string);
        TempData.Keep();

        var initialBookList = deserialized.Items;
        var bookDto = initialBookList.First(x => x.Id == bookId);

        ViewData["ReadersList"] = readers;
        ViewData["CurrentBookId"] = bookDto.Id;
        ViewData["CurrentBookTitle"] = bookDto.Title;
        ViewData["ShowBorrowPopup"] = true;
        ViewData["SearchTerm"] = search;

        return View("Index", new StaticPagedList<BookDto>(initialBookList, deserialized.PageNumber, deserialized.PageSize, deserialized.TotalItemCount));
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