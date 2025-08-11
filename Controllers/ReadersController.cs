using Application.Database;
using Application.Database.CustomExceptions;
using Application.Database.ReaderBooks;
using Application.Database.Readers;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
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
        var readers = await _readerRepository.GetPaginatedReadersFromDbAsync(search);

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

        await _readerRepository.UpdateReaderAsync(reader);
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
            TempData["AlertMessage"] = "reader obligatoriu";
            TempData["AlertType"] = "warning";
            return RedirectToAction(nameof(GetPaginatedReadersFromDb));
        }
        catch (InvalidEmailException)
        {
            TempData["AlertMessage"] = "Email invalid";
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

