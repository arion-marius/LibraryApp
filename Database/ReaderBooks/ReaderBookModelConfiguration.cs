using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Database.ReaderBooks;

public class ReaderBookModelConfiguration : IEntityTypeConfiguration<ReaderBookModel>
{
    public void Configure(EntityTypeBuilder<ReaderBookModel> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Reader)
            .WithMany(r => r.ReaderBooks)
            .HasForeignKey(x => x.ReaderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Book)
            .WithMany(b => b.Readers)
            .HasForeignKey(x => x.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.PickUpDate).IsRequired();
        builder.Property(x => x.ReturnedDate).HasColumnType("datetime2(3)"); ;
    }
}
