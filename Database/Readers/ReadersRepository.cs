using Application.Database.CustomExceptions;
using Application.Database.ReaderBooks;
using Application.Database.Readers;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Application.Database;

public class ReadersRepository : IReadersRepository
{
    private readonly LibraryDbContext _dbContext;

    public ReadersRepository(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ReaderSummaryDto>> GetReadersFromDbAsync()
    {
        var readers = await _dbContext.Readers.AsNoTracking()
            .Include(x => x.ReaderBooks)
            .Select(r => new ReaderSummaryDto
            {
                Id = r.Id,
                Name = r.Name,
                Email = r.Email,
                BooksBorrowed = r.BooksBorrowed,
                HasLateBooks = r.ReaderBooks.Any(rb => rb.ReturnedDate == null && rb.PickUpDate.AddMonths(1) < DateTime.Now)
            })
            .ToListAsync();

        return readers;
    }

    public async Task<List<ReaderSummaryDto>> GetPaginatedReadersFromDbAsync(int pageSize, int pageNumber)
    {
        var readers = await _dbContext.Readers
            .Include(x => x.ReaderBooks).ThenInclude(x => x.Book)
            .Select(r => new ReaderSummaryDto
            {
                Id = r.Id,
                Name = r.Name,
                Email = r.Email,
                BooksBorrowed = r.BooksBorrowed,
                HasLateBooks = r.ReaderBooks.Any(rb => rb.ReturnedDate == null && rb.PickUpDate.AddMonths(1) < DateTime.Now)
            })
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return readers;
    }

    public async Task<ReaderModel?> GetReaderByIdAsync(int id)
    {
        return await _dbContext.Readers.FindAsync(id);
    }

    public async Task UpdateReaderAsync(ReaderModel reader)
    {
        _dbContext.Readers.Update(reader);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<(bool success, string message)> DeleteReaderAsync(int id)
    {
        var reader = await _dbContext.Readers.FindAsync(id);

        bool hasBorrowedBooks = await _dbContext.ReaderBooks.AnyAsync(rb => rb.ReaderId == id);
        if (hasBorrowedBooks)
            return (false, $"Reader {reader.Name} cannot be deleted because they have books on loan.");

        _dbContext.Readers.Remove(reader);
        await _dbContext.SaveChangesAsync();

        return (true, $"Reader {reader.Name} was successfully deleted.");
    }

    public async Task<ReaderDto> GetReaderWithBooksByIdAsync(int id)
    {
        return await _dbContext.Readers
            .Include(r => r.ReaderBooks).ThenInclude(rb => rb.Book)
            .Where(x => x.Id == id)
            .Select(x => new ReaderDto
            {
                Id = id,
                Name = x.Name,
                Email = x.Email,
                BooksBorrowed = x.BooksBorrowed,
                Books = x.ReaderBooks.Select(rb => new BookDto
                {
                    Id = rb.BookId,
                    Title = rb.Book.Title,
                    Author = rb.Book.Author,
                    PickUpDate = rb.PickUpDate,
                    ReturnDate = rb.ReturnedDate,
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> HasReachedBorrowLimitAsync(int readerId)
    {
        return await _dbContext.ReaderBooks
            .CountAsync(rb => rb.ReaderId == readerId && rb.ReturnedDate == null) >= 5;
    }

    public async Task AddReaderBookAsync(int readerId, int bookId)
    {
        var readerBook = new ReaderBookModel
        {
            ReaderId = readerId,
            BookId = bookId,
            PickUpDate = DateTime.Now,
            ReturnedDate = null
        };

        _dbContext.ReaderBooks.Add(readerBook);
        await _dbContext.SaveChangesAsync();
    }

    //public void Test()
    //{
    //    var a = new List<int>();
    //    a.Add(7);

    //    var b = new List<string>();
    //    b.Add("");
    //    string asd = "marius";
    //    b.Add(asd);
    //    var c = new List<ReaderModel>();
    //    ReaderModel x = new()
    //    {
    //        Name = "a",
    //        Email = "b"
    //    };
    //    c.Add(x);

    //    var name = "name";
    //    var email = "email";
    //    var taxi = Create(name, email);

    //    var taxiList = new List<Taxi>();
    //    taxiList.Add(taxi);
    //    _dbContext.Readers.Add();

    //}

    public void Insert(string reader, string email)
    {
        var newReader = new ReaderModel { Name = reader, Email = email };
        _dbContext.Readers.Add(newReader);

        _dbContext.SaveChanges();
    }

    public async Task<ReaderDto> RemoveReaderBook(int readerId, int bookId)
    {
        var reader = await _dbContext.Readers
            .Include(x => x.ReaderBooks).ThenInclude(x => x.Book)
            .FirstOrDefaultAsync(x => x.Id == readerId);
        if (reader is null)
        {
            throw new ReaderNotFoundException();
        }

        var bookToBeReturned = reader.ReaderBooks.FirstOrDefault(x => x.BookId == bookId && !x.ReturnedDate.HasValue);
        if (bookToBeReturned is null)
        {
            throw new BookNotFoundException();
        }

        bookToBeReturned.ReturnedDate = DateTime.Now;
        reader.BooksBorrowed--;
        bookToBeReturned.Book.Stock++;

        await _dbContext.SaveChangesAsync();

        return new ReaderDto
        {
            Id = readerId,
            Name = reader.Name,
            Email = reader.Email,
            BooksBorrowed = reader.ReaderBooks.Count(x => !x.ReturnedDate.HasValue),
            Books = reader.ReaderBooks
                .Where(x => !x.ReturnedDate.HasValue)
                .Select(x => new BookDto
                {
                    Id = x.BookId,
                    Title = x.Book.Title,
                    Author = x.Book.Author,
                    PickUpDate = x.PickUpDate,
                    ReturnDate = null,
                })
                .ToList()
        };
    }
}