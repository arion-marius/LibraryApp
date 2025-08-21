 using Application.Database.Books;
using Application.Database.Readers;
using System;

namespace Application.Database.ReaderBooks;

public class ReaderBookModel
{
    public int Id { get; set; }
    public int ReaderId { get; set; }
    public ReaderModel Reader { get; set; }
    public int BookId { get; set; }
    public BookModel Book { get; set; }
    public DateTime PickUpDate { get; set; }
    public DateTime? ReturnedDate { get; set; }
}
