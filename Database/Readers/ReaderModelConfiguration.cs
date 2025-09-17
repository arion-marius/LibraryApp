using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Database.Readers;

public class ReaderModelConfiguration : IEntityTypeConfiguration<ReaderModel>
{
    public void Configure(EntityTypeBuilder<ReaderModel> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(ReaderModel.NameMaxLength)
            .IsUnicode(true);

        builder.Property(x => x.Email).IsRequired().HasMaxLength(ReaderModel.EmailMaxLength);
        builder.Property(x => x.BooksBorrowed);

        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasData(
            new ReaderModel { Id = 1, Name = "Andrei Durac", Email = "andrei@gmail.com" },
            new ReaderModel { Id = 2, Name = "Maria Ignat", Email = "maria@yahoo.com" },
            new ReaderModel { Id = 3, Name = "Ion Calinescu", Email = "ion.calinescu01@hotmail.com" },
            new ReaderModel { Id = 4, Name = "Elena Lasconi", Email = "elena.lasconi@gmail.com" },
            new ReaderModel { Id = 5, Name = "Mihai Mazare", Email = "MazareMih@yahoo.com" },
            new ReaderModel { Id = 6, Name = "Ana Ungureanu", Email = "anaxyz@gmail.com" },
            new ReaderModel { Id = 7, Name = "Cristian Branzescu", Email = "branzescu@gmail.com" },
            new ReaderModel { Id = 8, Name = "Ioana Dumitrascu", Email = "ioana001@gmail.com" },
            new ReaderModel { Id = 9, Name = "Vlad Ene", Email = "vlade999@yahoo.com" },
            new ReaderModel { Id = 10, Name = "Diana Agache", Email = "agache.dianamxn@gmail.com" }
);
    }
}
