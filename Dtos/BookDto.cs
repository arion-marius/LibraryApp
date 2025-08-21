using System;

namespace Application.Dtos;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public int Stock { get; set; }
    public DateTime PickUpDate { get; set; }
    public DateTime? ReturnDate { get; set; }
}
