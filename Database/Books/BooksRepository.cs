using Application.Database.CustomExceptions;
using Application.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace Application.Database.Books;

public class BooksRepository : IBooksRepository
{
    private readonly LibraryDbContext _dbContext;
    public BooksRepository(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public PagedList<BookDto> GetPagedBooks(string search, int pageNumber = 1, int pageSize = 5)
    {
        var books = _dbContext.Books
            .AsNoTracking()
            .Where(b => string.IsNullOrEmpty(search) ? true : b.Title.Contains(search))
            .Select(book => new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Stock = book.Stock,
            });
        var pagedList = new PagedList<BookDto>(books, pageNumber, pageSize);
        return pagedList;
    }
    public async Task<List<BookDto>> GetBooksAsync(string search)
    {
        var books = await _dbContext.Books
            .AsNoTracking()
            .Where(b => string.IsNullOrEmpty(search) ? true : b.Title.Contains(search))
            .Select(book => new BookDto()
            {
                Title = book.Title,
                Author = book.Author,
            })
            .ToListAsync();
        return books;
    }
    public async Task<(bool success, string message)> DeleteBookAsync(int bookId)
    {
        var book = await _dbContext.Books.Include(x => x.Readers).FirstOrDefaultAsync(x => x.Id == bookId);
        if (book.Readers.Any(x => x.ReturnedDate == null))
            return (false, $"The book {book.Title} cannot be deleted because it s borrowed.");

        _dbContext.Books.Remove(book);
        await _dbContext.SaveChangesAsync();

        return (true, $"The book {book.Title} was succesfully deleted.");
    }
    public async Task<BookDto> GetBookByIdAsync(int id)
    {
        return await _dbContext.Books
            .AsNoTracking()
            .Where(book => book.Id == id)
            .Select(book => new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Stock = book.Stock,
            })
            .FirstOrDefaultAsync();
    }
    public async Task UpdateBookAsync(BookModel book)
    {
        BookValidator.TryValidate(book.Author, book.Title);
        if (_dbContext.Books.Any(x => x.Id != book.Id && x.Author == book.Author && x.Title == book.Title))
        {
            throw new BookAlreadyExistException();
        }

        _dbContext.Books.Update(book);
        await _dbContext.SaveChangesAsync();
    }
    public async Task AddBookAsync(BookModel book)
    {
        BookValidator.TryValidate(book.Author, book.Title);
        if (_dbContext.Books.Any(x => x.Id != book.Id && x.Author == book.Author && x.Title == book.Title))
        {
            throw new BookAlreadyExistException();
        }

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