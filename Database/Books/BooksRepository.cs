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

    //public Task<List<BookModel>> GetBooksAsync()
    //{
    //    return Task.FromResult(
    //        new List<BookModel>
    //        {
    //            //new() { Id = 1, Title = "1984", Author = "George Orwell", Stock = 10 },
    //            //new() { Id = 2, Title = "To Kill a Mockingbird", Author = "Harper Lee", Stock = 10},
    //            //new() { Id = 3, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", Stock = 10, BorrowedCount = 0 },
    //            //new() { Id = 4, Title = "To Kill a Mockingbird", Author = "Harper Lee" , Stock = 10, BorrowedCount = 0},
    //            //new() { Id = 5, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald" , Stock = 10, BorrowedCount = 0},
    //            //new() { Id = 6, Title = "To Kill a Mockingbird", Author = "Harper Lee" , Stock = 10, BorrowedCount = 0},
    //            //new() { Id = 7, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald" , Stock = 10, BorrowedCount = 0},
    //            //new() { Id = 8, Title = "To Kill a Mockingbird", Author = "Harper Lee"   , Stock = 10, BorrowedCount = 0},
    //            //new() { Id = 9, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald" , Stock = 10, BorrowedCount = 0},
    //            //new() { Id = 2, Title = "To Kill a Mockingbird", Author = "Harper Lee" , Stock = 10, BorrowedCount = 0},
    //            //new() { Id = 3, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald" , Stock = 10, BorrowedCount = 0},
    //            //new() { Id = 4, Title = "To Kill a Mockingbird", Author = "Harper Lee" , Stock = 10, BorrowedCount = 0},
    //            //new() { Id = 5, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald"    , Stock = 10, BorrowedCount = 0},
    //            //new() { Id = 6, Title = "To Kill a Mockingbird", Author = "Harper Lee" , Stock = 10, BorrowedCount = 0},
    //            //new() { Id = 7, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald" , Stock = 10, BorrowedCount = 0},
    //            //new() { Id = 8, Title = "To Kill a Mockingbird", Author = "Harper Lee" , Stock = 10, BorrowedCount = 0},
    //            //new() { Id = 9, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald" , Stock = 10, BorrowedCount = 0},
    //            //new() { Id = 1, Title = "1984", Author = "George Orwell" , Stock = 10, BorrowedCount = 0},
    //            //new() { Id = 2, Title = "To Kill a Mockingbird", Author = "Harper Lee" , Stock = 10, BorrowedCount = 0},
    //            //new() { Id = 3, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald" , Stock = 10, BorrowedCount = 0},
    //            //new() { Id = 4, Title = "To Kill a Mockingbird", Author = "Harper Lee" , Stock = 10, BorrowedCount = 0},
    //            //new() { Id = 5, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald" , Stock = 10  , BorrowedCount = 0},
    //            //new() { Id = 6, Title = "To Kill a Mockingbird", Author = "Harper Lee" , Stock = 10, BorrowedCount = 0},
    //            //new() { Id = 7, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald" , Stock = 10, BorrowedCount = 0},
    //            //new() { Id = 8, Title = "To Kill a Mockingbird", Author = "Harper Lee" , Stock = 10, BorrowedCount = 0},
    //            //new() { Id = 9, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald" , Stock = 10, BorrowedCount = 0}
    //        });
    //}

    public async Task<List<BookModel>> GetBooksFromDbAsync()
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

}