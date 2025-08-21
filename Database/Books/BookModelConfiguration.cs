using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Database.Books;

public class BookModelConfiguration : IEntityTypeConfiguration<BookModel>
{
    public void Configure(EntityTypeBuilder<BookModel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(BookModel.TitleMaxLength);
        builder.Property(x => x.Author).IsRequired().HasMaxLength(BookModel.AuthorMaxLength);
    }
}
