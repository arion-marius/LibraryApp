using Application.Database.ReaderBooks;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Application.Database.Readers;

public interface IReadersRepository
{
    Task<ReaderDto?> GetReaderByIdAsync(int id);
    Task UpdateReaderAsync(ReaderDto reader);
    Task<ReaderDto?> GetReaderWithBooksByIdAsync(int id);
    Task<(bool success, string message)> DeleteReaderAsync(int id);
    Task<List<ReaderSummaryDto>> GetPaginatedReadersFromDbAsync(string search);
    void Insert(string name, string email);
    Task<ReaderDto> RemoveReaderBook(int readerId, int bookId);
}
