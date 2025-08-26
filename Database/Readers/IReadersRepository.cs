using Application.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace Application.Database.Readers;

public interface IReadersRepository
{
    Task<ReaderDto?> GetReaderByIdAsync(int id);
    Task UpdateReaderAsync(ReaderDto reader);
    Task<ReaderDto?> GetReaderWithBooksByIdAsync(int id);
    Task<(bool success, string message)> DeleteReaderAsync(int id);
    Task<List<ReaderPopUpDto>> GetTop20ReadersAsync(string search);
    void Insert(string name, string email);
    Task<ReaderDto> RemoveReaderBook(int readerId, int bookId);
    IPagedList<ReaderSummaryDto> GetPagedReaders(string search, int? page);
}
