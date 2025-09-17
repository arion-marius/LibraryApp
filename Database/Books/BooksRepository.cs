using Application.Database.CustomExceptions;
using Application.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace Application.Database.Books;

public class BooksRepository(LibraryDbContext dbContext) : IBooksRepository
{
    private readonly LibraryDbContext _dbContext = dbContext;

    public PagedList<BookDto> GetPagedBooks(string search = "", int pageNumber = 1, int pageSize = 5)
    {
        bool containsDiacritics = search.Any(c => "ăâîșțĂÂÎȘȚ".Contains(c));

        IQueryable<BookDto> booksQueryable;
        if (!containsDiacritics)
        {
            var normalizedSearch = StringHelper.Normalize(search).ToLower();
            booksQueryable = _dbContext.Books
                .AsNoTracking()
                .OrderBy(b => b.NormalizedTitle)
                .Where(b => string.IsNullOrEmpty(normalizedSearch) || b.NormalizedTitle.Contains(normalizedSearch))
                .Select(book => new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    Stock = book.Stock,
                });
        }
        else
        {
            booksQueryable = _dbContext.Books
                 .AsNoTracking()
                 .OrderBy(b => b.Title)
                 .Where(b => string.IsNullOrEmpty(search) || EF.Functions.Collate(b.Title.ToLower(), "Romanian_100_CI_AS").Contains(search.ToLower()))
                 .Select(book => new BookDto
                 {
                     Id = book.Id,
                     Title = book.Title,
                     Author = book.Author,
                     Stock = book.Stock
                 });
    }
        return new PagedList<BookDto>(booksQueryable, pageNumber, pageSize);
    }
    public async Task<List<BookDto>> GetBooksAsync(string search)
    {
        var normalizedSearch = StringHelper.Normalize(search);
        var books = await _dbContext.Books
            .AsNoTracking()
           .Where(b => string.IsNullOrEmpty(normalizedSearch) ? true : b.NormalizedTitle.Contains(normalizedSearch))
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
    public async Task UpdateBookAsync(BookDto book)
    {
        BookValidator.TryValidate(book.Author, book.Title);
        if (_dbContext.Books.Any(x => x.Id != book.Id && x.Author == book.Author && x.Title == book.Title))
        {
            throw new BookAlreadyExistException();
        }

        var bookDto = await _dbContext.Books.FindAsync(book.Id);
        bookDto.Title = book.Title;
        bookDto.Author = book.Author;
        bookDto.Stock = book.Stock;
        bookDto.NormalizedTitle = StringHelper.Normalize(book.Title);

        await _dbContext.SaveChangesAsync();
    }
    public async Task AddBookAsync(BookDto book)
    {
        BookValidator.TryValidate(book.Author, book.Title);
        if (_dbContext.Books.Any(x => x.Author == book.Author && x.Title == book.Title))
        {
            throw new BookAlreadyExistException();
        }

        var bookModel = new BookModel
        {
            Title = book.Title,
            Author = book.Author,
            Stock = book.Stock,
            NormalizedTitle = StringHelper.Normalize(book.Title)
        };
        _dbContext.Books.Add(bookModel);
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
public static class StringHelper
{
    public static string Normalize(string input)
    {
        if (input == null) return null;

        var normalized = input.Normalize(NormalizationForm.FormD);

        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
    }
}

