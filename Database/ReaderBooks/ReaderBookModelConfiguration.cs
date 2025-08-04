using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Database.ReaderBooks;

public class ReaderBookModelConfiguration : IEntityTypeConfiguration<ReaderBookModel>
{
    public void Configure(EntityTypeBuilder<ReaderBookModel> builder)
    {
        builder.HasKey(x => new { x.ReaderId, x.BookId });

        builder.HasOne(x => x.Reader)
            .WithMany(r => r.ReaderBooks)
            .HasForeignKey(x => x.ReaderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Book)
            .WithMany(b => b.Readers)
            .HasForeignKey(x => x.BookId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
