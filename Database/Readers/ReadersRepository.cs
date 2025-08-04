using Application.Database.ReaderBooks;
using Application.Database.Readers;
using Application.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
        var readers = await _dbContext.Readers
            .Select(r => new ReaderSummaryDto
            {
                Id = r.Id,
                Name = r.Name,
                Email = r.Email,
                BooksBorrowed = r.ReaderBooks.Count(),
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
                BooksBorrowed = r.BooksBorrowed ?? 0,
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
        var book = _dbContext.Books.FirstOrDefaultAsync(x => x.Id == bookId);

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


}
