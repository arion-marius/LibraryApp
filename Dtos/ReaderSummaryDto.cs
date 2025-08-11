namespace Application.Dtos;

public class ReaderSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int BooksBorrowed { get; set; }
    public string Email { get; set; }

    public bool HasLateBooks { get; set; }
}
