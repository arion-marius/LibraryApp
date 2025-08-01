using Application.Database.Books;
using Application.Database.ReaderBooks;
using Application.Database.Readers;
using Microsoft.EntityFrameworkCore;

namespace Application.Database;

public class LibraryDbContext(DbContextOptions<LibraryDbContext> options) : DbContext(options)
{
    public DbSet<BookModel> Books { get; set; }
    public DbSet<ReaderModel> Readers { get; set; }
    public DbSet<ReaderBookModel> ReaderBooks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new BookModelConfiguration());
        builder.ApplyConfiguration(new ReaderModelConfiguration());
        builder.ApplyConfiguration(new ReaderBookModelConfiguration());
    }
}