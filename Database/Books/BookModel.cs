using Application.Database.ReaderBooks;
using System.Collections.Generic;

namespace Application.Database.Books;

public class BookModel
{
    public const int TitleMaxLength = 200;
    public const int AuthorMaxLength = 100;
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public int Stock { get; set; }
    public ICollection<ReaderBookModel> Readers { get; set; } = [];
}
