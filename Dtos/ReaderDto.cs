using System.Collections.Generic;

namespace Application.Dtos;

public class ReaderDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? BooksBorrowed { get; set; }
    public string Email { get; set; }

    public ICollection<BookDto> Books { get; set; }
}
