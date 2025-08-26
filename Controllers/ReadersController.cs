using Application.Database.CustomExceptions;
using Application.Database.Readers;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace Application.Controllers;

public class ReadersController : Controller
{
    private readonly IReadersRepository _readerRepository;

    public ReadersController(IReadersRepository readerRepository)
    {
        _readerRepository = readerRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaginatedReadersFromDb(string? search, int? page)
    {
        var readers = await _readerRepository.GetTop20ReadersAsync(search);

        ViewData["Action"] = nameof(GetPaginatedReadersFromDb);
        ViewData["Search"] = search;

        return View("Index", readers.ToPagedList(page ?? 1, 5));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var reader = await _readerRepository.GetReaderByIdAsync(id);
        if (reader == null)
            return NotFound();

        return View(reader);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ReaderDto reader)
    {
        if (!ModelState.IsValid)
            return View(reader);

        try
        {
            await _readerRepository.UpdateReaderAsync(reader);
        }
        catch (ReaderNotFoundException)
        {
            TempData["AlertMessage"] = "reader is required";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPaginatedReadersFromDb));
        }
        catch (InvalidEmailException)
        {
            TempData["AlertMessage"] = "email is required";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPaginatedReadersFromDb));
        }
        catch (UsedEmailException)
        {
            TempData["AlertMessage"] = "The email is already in use.";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPaginatedReadersFromDb));
        }
        catch (EmailMaxLengthException)
        {
            TempData["AlertMessage"] = "The email exceeds 254 characters";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPaginatedReadersFromDb));
        }
        catch (ReaderMaxLenghtException)
        {
            TempData["AlertMessage"] = "The name of the reader exceeds 100 characters";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPaginatedReadersFromDb));
        }
        catch (InvalidReaderException)
        {
            TempData["AlertMessage"] = "Invalid Reader Name";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPaginatedReadersFromDb));
        }

        TempData["AlertMessage"] = $"Reader {reader.Name} has been modified.";
        TempData["AlertType"] = "success";

        return RedirectToAction(nameof(GetPaginatedReadersFromDb));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var reader = await _readerRepository.GetReaderWithBooksByIdAsync(id);
        if (reader == null)
            return NotFound();

        return View(reader);
    }


    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _readerRepository.DeleteReaderAsync(id);

        TempData["AlertMessage"] = message;
        TempData["AlertType"] = success ? "success" : "warning";

        return RedirectToAction(nameof(GetPaginatedReadersFromDb));
    }

    [HttpGet]
    public async Task<IActionResult> History(int id)
    {
        var reader = await _readerRepository.GetReaderWithBooksByIdAsync(id);
        if (reader == null)
            return NotFound();

        return View(reader);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(string reader, string? email)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        try
        {
            _readerRepository.Insert(reader, email);
        }
        catch (ReaderNotFoundException)
        {
            TempData["AlertMessage"] = "Reader is required";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPaginatedReadersFromDb));
        }
        catch (InvalidEmailException)
        {
            TempData["AlertMessage"] = "Email invalid";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPaginatedReadersFromDb));
        }
        catch (UsedEmailException)
        {
            TempData["AlertMessage"] = "The email is already in use.";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPaginatedReadersFromDb));
        }
        catch (EmailMaxLengthException)
        {
            TempData["AlertMessage"] = "The email exceeds 254 characters";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPaginatedReadersFromDb));
        }
        catch (ReaderMaxLenghtException)
        {
            TempData["AlertMessage"] = "The name of the reader exceeds 100 characters";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPaginatedReadersFromDb));
        }
        catch (InvalidReaderException)
        {
            TempData["AlertMessage"] = "Invalid Reader Name";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPaginatedReadersFromDb));
        }

        return RedirectToAction(nameof(GetPaginatedReadersFromDb));
    }

    [HttpPost]
    public async Task<IActionResult> ReturnBook(int readerId, int bookId)
    {
        if (readerId == 0 || bookId == 0)
        {
            return RedirectToAction(nameof(GetPaginatedReadersFromDb));
        }

        var readerDto = await _readerRepository.RemoveReaderBook(readerId, bookId);

        return View(nameof(Details), readerDto);
    }
}
