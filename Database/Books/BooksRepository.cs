using Application.Database.CustomExceptions;
using Application.Database.ReaderBooks;
using Application.Database.Readers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Database.Books;

public class BooksRepository : IBooksRepository
{
    private readonly LibraryDbContext _dbContext;

    public BooksRepository(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<BookModel>> GetBooksAsync()
    {
        var books = await _dbContext.Books.ToListAsync();
        return books;
    }

    public async Task<(bool success, string message)> DeleteBookAsync(int id)
    {
        var book = await _dbContext.Books.FindAsync(id);
        bool x = await _dbContext.ReaderBooks.AnyAsync(rb => rb.BookId == id);
        if (x)
            return (false, $"The book {book.Title} cannot be deleted because it is on loan.");

        _dbContext.Books.Remove(book);
        await _dbContext.SaveChangesAsync();

        return (true, $"The book {book.Title} was succesfully deleted.");
    }

    public async Task<BookModel> GetBookByIdAsync(int id)
    {
        return await _dbContext.Books.FindAsync(id);
    }

    public async Task UpdateBookAsync(BookModel book)
    {
        _dbContext.Books.Update(book);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddBookAsync(BookModel book)
    {
        _dbContext.Books.Add(book);
        await _dbContext.SaveChangesAsync();
    }

    public async Task BorrowAsync(int bookId, int readerId)
    {
        var book = await _dbContext.Books.FirstOrDefaultAsync(x => x.Id == bookId);
        if (book == null)
        {
            throw new BookNotFoundException();
        }
        else if (book.Stock == 0)
        {
            throw new BookOutOfStockException();
        }

        var reader = await _dbContext.Readers.Include(r => r.ReaderBooks).FirstOrDefaultAsync(x => x.Id == readerId);
        if (reader == null)
        {
            throw new ReaderNotFoundException();
        }
        if (reader.BooksBorrowed >= 5)
        {
            throw new TooManyBooksException();
        }
        if (reader.ReaderBooks.Any(x => x.BookId == book.Id && !x.ReturnedDate.HasValue))
        {
            throw new BookAlreadyBorrowedException();
        }

        reader.ReaderBooks.Add(new()
        {
            BookId = bookId,
            ReaderId = readerId,
            PickUpDate = DateTime.Now,
        });
        reader.BooksBorrowed++;
        book.Stock--;
        await _dbContext.SaveChangesAsync();

    }
}