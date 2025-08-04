using Application.Database.ReaderBooks;
using Application.Database.Readers;
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

    public async Task<List<ReaderModel>> GetReadersFromDbAsync()
    {
        var readers = await _dbContext.Readers
            .Select(r => new ReaderModel
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

    public async Task<ReaderModel> GetReaderWithBooksByIdAsync(int id)
    {
        return await _dbContext.Readers
            .Include(r => r.ReaderBooks)
                .ThenInclude(rb => rb.Book)
            .FirstOrDefaultAsync(r => r.Id == id);
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


}
