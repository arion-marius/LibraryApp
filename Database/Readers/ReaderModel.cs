using Application.Database.ReaderBooks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Database.Readers;

public class ReaderModel
{
    public const int NameMaxLength = 100;

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? BooksBorrowed { get; set; } 
    public string Email { get; set; } = string.Empty;

    [NotMapped]
    public bool HasLateBooks { get; set; }

    public IEnumerable<ReaderBookModel> ReaderBooks { get; set; } = new List<ReaderBookModel>();
}
