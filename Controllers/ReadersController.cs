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
    public IActionResult GetPagedReaders(string? search, int? page = 1)
    {
        var readers = _readerRepository.GetPagedReaders(search, page);

        ViewData["Search"] = search;

        return View("Index", readers);
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
            return RedirectToAction(nameof(GetPagedReaders));
        }
        catch (InvalidEmailException)
        {
            TempData["AlertMessage"] = "email is required";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPagedReaders));
        }
        catch (UsedEmailException)
        {
            TempData["AlertMessage"] = "The email is already in use.";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPagedReaders));
        }
        catch (EmailMaxLengthException)
        {
            TempData["AlertMessage"] = "The email exceeds 254 characters";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPagedReaders));
        }
        catch (ReaderMaxLenghtException)
        {
            TempData["AlertMessage"] = "The name of the reader exceeds 100 characters";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPagedReaders));
        }
        catch (InvalidReaderException)
        {
            TempData["AlertMessage"] = "Invalid Reader Name";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPagedReaders));
        }

        TempData["AlertMessage"] = $"Reader {reader.Name} has been modified.";
        TempData["AlertType"] = "success";

        return RedirectToAction(nameof(GetPagedReaders));
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

        return RedirectToAction(nameof(GetPagedReaders));
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
            return RedirectToAction(nameof(GetPagedReaders));
        }
        catch (InvalidEmailException)
        {
            TempData["AlertMessage"] = "Email invalid";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPagedReaders));
        }
        catch (UsedEmailException)
        {
            TempData["AlertMessage"] = "The email is already in use.";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPagedReaders));
        }
        catch (EmailMaxLengthException)
        {
            TempData["AlertMessage"] = "The email exceeds 254 characters";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPagedReaders));
        }
        catch (ReaderMaxLenghtException)
        {
            TempData["AlertMessage"] = "The name of the reader exceeds 100 characters";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPagedReaders));
        }
        catch (InvalidReaderException)
        {
            TempData["AlertMessage"] = "Invalid Reader Name";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPagedReaders));
        }

        return RedirectToAction(nameof(GetPagedReaders));
    }

    [HttpPost]
    public async Task<IActionResult> ReturnBook(int readerId, int bookId)
    {
        if (readerId == 0 || bookId == 0)
        {
            return RedirectToAction(nameof(GetPagedReaders));
        }

        var readerDto = await _readerRepository.RemoveReaderBook(readerId, bookId);

        return View(nameof(Details), readerDto);
    }
}
