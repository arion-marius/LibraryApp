using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Database.Readers;

public class ReaderModelConfiguration : IEntityTypeConfiguration<ReaderModel>
{
    public void Configure(EntityTypeBuilder<ReaderModel> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(ReaderModel.NameMaxLength);
        builder.Property(x => x.BooksBorrowed);

        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();
    }
}
