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

        builder.HasData(
            new BookModel { Id = 1, Author = "Mihail Sadoveanu", Title = "Baltagul", Stock = 99},
            new BookModel { Id = 2, Author = "Marin Preda", Title = "Moromeții", Stock = 99 },
            new BookModel { Id = 3, Author = "George Călinescu", Title = "Enigma Otiliei" , Stock = 99 },
            new BookModel { Id = 4, Author = "Liviu Rebreanu", Title = "Ion" , Stock = 99 },
            new BookModel { Id = 5, Author = "Camil Petrescu", Title = "Ultima noapte de dragoste, întâia noapte de război" , Stock = 99 },
            new BookModel { Id = 6, Author = "Ion Creangă", Title = "Amintiri din copilărie" , Stock = 99 },
            new BookModel { Id = 7, Author = "Mateiu Caragiale", Title = "Craii de Curtea-Veche" , Stock = 99 },
            new BookModel { Id = 8, Author = "Liviu Rebreanu", Title = "Pădurea spânzuraților" , Stock = 99 },
            new BookModel { Id = 9, Author = "Mircea Eliade", Title = "Scrinul negru" , Stock = 99 },
            new BookModel { Id = 10, Author = "Marin Preda", Title = "Cel mai iubit dintre pământeni" , Stock = 99 }
        );
    }
}
