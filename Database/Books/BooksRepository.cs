using Application.Database.CustomExceptions;
using Application.Database.ReaderBooks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
        bool pulamea = await _dbContext.ReaderBooks.AnyAsync(rb => rb.BookId == id);
        if (pulamea)
            return (false, $"The book \"{{book.Title}}\" cannot be deleted because it is on loan.");

        _dbContext.Books.Remove(book);
        await _dbContext.SaveChangesAsync();

        return (true, $"The book \"{{book.Title}}\" was successfully deleted.");
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

        var reader = await _dbContext.Readers.FirstOrDefaultAsync(x => x.Id == readerId);
        if (reader == null)
        {
            throw new ReaderNotFoundException();
        }

        if (reader.BooksBorrowed >= 5)
        {
            throw new TooManyBooksException();
        }

        reader.BooksBorrowed++;
        reader.ReaderBooks.Add(new()
        {
            BookId = bookId,
            ReaderId = readerId,
            PickUpDate = DateTime.Now,
        });

        await _dbContext.SaveChangesAsync();
    }
}