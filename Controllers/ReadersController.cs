using Application.Database.ReaderBooks;
using Application.Database.Readers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace Application.Controllers;

public class ReadersController(IReadersRepository repository) : Controller
{
    private readonly IReadersRepository _readerRepository = repository;

    [HttpGet]
    public async Task<IActionResult> GetReadersFromDb(string? search, int? page)
    {
        var readers = await _readerRepository.GetReadersFromDbAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            readers = readers
                .Where(r => r.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        int pageSize = 5;
        int pageNumber = page ?? 1;

        var pagedReaders = readers.ToPagedList(pageNumber, pageSize);
        ViewData["Action"] = "GetReadersFromDb";
        ViewData["Search"] = search;

        return View("Index", pagedReaders);
    }


    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var reader = await _readerRepository.GetReaderByIdAsync(id);
        if (reader == null)
            return NotFound();

        return View(reader);
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
    public async Task<IActionResult> Edit(ReaderModel reader)
    {
        if (!ModelState.IsValid)
            return View(reader);

        await _readerRepository.UpdateReaderAsync(reader);
        TempData["AlertMessage"] = $"Reader \"{{reader.Name}}\" has been modified.";
        TempData["AlertType"] = "success";

        return RedirectToAction(nameof(GetReadersFromDb));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _readerRepository.DeleteReaderAsync(id);

        TempData["AlertMessage"] = message;
        TempData["AlertType"] = success ? "success" : "warning";

        return RedirectToAction(nameof(GetReadersFromDb));
    }

    [HttpGet]
    public async Task<IActionResult> History(int id)
    {
        var reader = await _readerRepository.GetReaderWithBooksByIdAsync(id);
        if (reader == null)
            return NotFound();

        return View(reader);
    }

    [HttpPost]
    public async Task<IActionResult> Borrow(int readerId, int bookId, string? cancel)
    {
        if (!string.IsNullOrEmpty(cancel))
        {
            return RedirectToAction(nameof(GetReadersFromDb));
        }

        if (await _readerRepository.HasReachedBorrowLimitAsync(readerId))
        {
            TempData["AlertMessage"] = "Reader has already borrowed 5 books that are not yet returned.";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetReadersFromDb));
        }

        await _readerRepository.AddReaderBookAsync(readerId, bookId);

        TempData["AlertMessage"] = "Book was successfully borrowed.";
        TempData["AlertType"] = "success";
        return RedirectToAction(nameof(GetReadersFromDb));
    }

}
