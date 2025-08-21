using Application.Database.ReaderBooks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.Database.Readers;

public class ReaderModel
{
    public const int NameMaxLength = 100;
    public const int EmailMaxLength = 254;
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int BooksBorrowed { get; set; } 
    public string Email { get; set; } = string.Empty;
    public ICollection<ReaderBookModel> ReaderBooks { get; set; } = [];
}
    