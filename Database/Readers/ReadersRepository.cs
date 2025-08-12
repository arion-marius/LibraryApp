using Application.Database.CustomExceptions;
using Application.Database.Readers;
using Application.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Database;

public class ReadersRepository : IReadersRepository
{
    private readonly LibraryDbContext _dbContext;

    public ReadersRepository(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ReaderSummaryDto>> GetPaginatedReadersFromDbAsync(string search)
    {
        var readers = await _dbContext.Readers
            .Include(x => x.ReaderBooks).ThenInclude(x => x.Book)
            .Where(r => string.IsNullOrWhiteSpace(search) ? true : r.Name.Contains(search))
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

    public async Task<ReaderDto?> GetReaderByIdAsync(int id)
    {
        return await _dbContext.Readers
            .Where(x => x.Id == id)
            .Select(x => new ReaderDto
            {
                Id = id,
                Name = x.Name,
                Email = x.Email,
                BooksBorrowed = x.BooksBorrowed,
            })
            .FirstOrDefaultAsync();
    }

    public async Task UpdateReaderAsync(ReaderDto reader)
    {
        var readerModel = await _dbContext.Readers.FindAsync(reader.Id);
        var test = _dbContext.Readers.Any(x => x.Email == reader.Email);
        var emailvalid = IsValidEmail(reader.Email);
        var readeremail = readerModel.Email != reader.Email;
        if (string.IsNullOrWhiteSpace(reader.Name))
        {
            throw new ReaderNotFoundException();
        }
        else if (readerModel.Email != reader.Email &&
           IsValidEmail(reader.Email)
            && _dbContext.Readers.Any(x => x.Email == reader.Email))
        {
            throw new UsedEmailException();
        }
        else if (string.IsNullOrWhiteSpace(reader.Email))
        {
            throw new InvalidEmailException();
        }

        readerModel.Name = reader.Name;
        readerModel.Email = reader.Email;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<(bool success, string message)> DeleteReaderAsync(int id)
    {
        var reader = await _dbContext.Readers.FindAsync(id);

        if (reader.BooksBorrowed > 0)
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

    public void Insert(string reader, string email)
    {
        var newReader = new ReaderModel { Name = reader, Email = email };

        if (string.IsNullOrWhiteSpace(reader))
        {
            throw new ReaderNotFoundException();
        }
        else if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
        {
            throw new InvalidEmailException();
        }

        _dbContext.Readers.Add(newReader);
        _dbContext.SaveChanges();
    }

    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.IgnoreCase);
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