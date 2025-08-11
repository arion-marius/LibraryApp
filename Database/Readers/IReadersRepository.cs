using Application.Database.ReaderBooks;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Application.Database.Readers;

public interface IReadersRepository
{
    Task<ReaderModel?> GetReaderByIdAsync(int id);
    Task UpdateReaderAsync(ReaderModel reader);
    Task<ReaderDto?> GetReaderWithBooksByIdAsync(int id);
    Task<bool> HasReachedBorrowLimitAsync(int readerId);
    Task<(bool success, string message)> DeleteReaderAsync(int id);
    Task AddReaderBookAsync(int readerId, int bookId);
    Task<List<ReaderSummaryDto>> GetPaginatedReadersFromDbAsync(string search);
    void Insert(string name, string email);
    Task<ReaderDto> RemoveReaderBook(int readerId, int bookId);
}
