using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Application.Database.Books;

public class BookModelConfiguration : IEntityTypeConfiguration<BookModel>
{
    public void Configure(EntityTypeBuilder<BookModel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(BookModel.TitleMaxLength)
            .IsUnicode(true);

        builder.Property(x => x.Author)
            .IsRequired()
            .HasMaxLength(BookModel.AuthorMaxLength)
            .IsUnicode(true);

        builder.Property(b => b.NormalizedTitle)
            .IsRequired()
            .HasMaxLength(BookModel.TitleMaxLength);

        builder.HasData(SeedBooks());
    }

    public static ICollection<BookModel> SeedBooks()
    {
        return
        [
            new() { Id = 1, Author = "Mihail Sadoveanu", Title = "Baltagul", Stock = 99, NormalizedTitle = "Baltagul"},
            new() { Id = 2, Author = "Marin Preda", Title = "Moromeții", Stock = 99, NormalizedTitle = "Morometii" },
            new() { Id = 3, Author = "George Călinescu", Title = "Enigma Otiliei", Stock = 99, NormalizedTitle="Enigma Otiliei"},
            new() { Id = 4, Author = "Liviu Rebreanu", Title = "Ion", Stock = 99, NormalizedTitle = "Ion" },
            new() { Id = 5, Author = "Camil Petrescu", Title = "Ultima noapte de dragoste, întâia noapte de război", Stock = 99, NormalizedTitle = "Ultima noapte de dragoste, intaia noapte de razboi" },
            new() { Id = 6, Author = "Ion Creangă", Title = "Amintiri din copilărie", Stock = 99 , NormalizedTitle = "Amintiri din copilarie"},
            new() { Id = 7, Author = "Mateiu Caragiale", Title = "Craii de Curtea-Veche", Stock = 99 , NormalizedTitle = "Craii de Curtea-Veche"},
            new() { Id = 8, Author = "Liviu Rebreanu", Title = "Pădurea spânzuraților", Stock = 99 , NormalizedTitle = "Padurea spanzuratilor"},
            new() { Id = 9, Author = "Mircea Eliade", Title = "Scrinul negru", Stock = 99 , NormalizedTitle = "Scrinul negru"},
            new() { Id = 10, Author = "Marin Preda", Title = "Cel mai iubit dintre pământeni", Stock = 99 , NormalizedTitle = "Cel mai iubit dintre pamanteni"}
        ];
    }
}
