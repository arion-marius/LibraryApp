using Application.Database;
using Application.Database.Readers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace Application.Controllers;

public class ReadersController(IReadersRepository repository) : Controller
{
    private readonly IReadersRepository _repository = repository;

    [HttpGet]
    public async Task<IActionResult> GetReadersFromDb(string? search, int? page)
    {
        var readers = await _repository.GetReadersFromDbAsync();

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
        var reader = await _repository.GetReaderByIdAsync(id);
        if (reader == null)
            return NotFound();

        return View(reader);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var reader = await _repository.GetReaderWithBooksByIdAsync(id);
        if (reader == null)
            return NotFound();

        return View(reader);

        if (reader == null)
            return NotFound();

        return View(reader);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ReaderModel reader)
    {
        if (!ModelState.IsValid)
            return View(reader);

        await _repository.UpdateReaderAsync(reader);
        TempData["AlertMessage"] = $"Reader \"{{reader.Name}}\" has been modified.";
        TempData["AlertType"] = "success";

        return RedirectToAction(nameof(GetReadersFromDb));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _repository.DeleteReaderAsync(id);

        TempData["AlertMessage"] = message;
        TempData["AlertType"] = success ? "success" : "warning";

        return RedirectToAction(nameof(GetReadersFromDb));
    }

}
