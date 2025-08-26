# library

## LibraryApp

An **ASP.NET Core MVC application** for managing books and readers.  
It provides CRUD operations, search, pagination, borrowing/returning books with validations who are using custom exceptions.  

---

## 1. Listing all books in the library, indicating whether they are available and number of the stock.
## 2. Adding new books + new readers to the library.
## 3. Facilitating the lending of books to users, capturing the borrower's details and return date.
## 4. Enabling users to borrow books and return books to the library.
## 5. Displaying due dates and borrower information when listing books that are currently on loan.

**For the start you have seeded 10 books and 10 readers by default. If you want to delete them,
enter in BookModelConfiguration/ReaderModelConfiguration and delete de part with builder.HasData**

### Bulk insert
 **100k books and readers** 
 via SQL seed script(if you want to use it, go to Database -> DatabaseSeed100k.SQL, copy, open SQL and paste it in your SQLQuery.
 If you want to delete all data for the moment, open SQL and write in SQLQuery these 2 commands: DELETE FROM Readers WHERE Id > 0
          DELETE FROM Books WHERE Id > 0), if you close the application, the database returns to its default form.

### Bulk insert
 **100k books and readers** 
 via SQL seed script(if you want to use it, go to Database -> DatabaseSeed100k.SQL, copy, open SQL and paste it in your SQLQuery.
 If you want to delete all data for the moment, open SQL and write in SQLQuery these 2 commands: DELETE FROM Readers WHERE Id > 0
          DELETE FROM Books WHERE Id > 0), if you close the application, the database returns to its default form.

##  Tech Stack
- **DOT .NET:** 9.0
- **Backend:** ASP.NET Core MVC (C#)
- **Database:** SQL Server + Entity Framework Core
- **UI:** Razor Views + Bootstrap
- **Pagination:** X.PagedList
- **Serialization:** System.Text.Json

## ⚙️ Installation & Setup

1. **Clone repository**
   git clone https://github.com/arion-marius/LibraryApp.git
   cd LibraryManagementSystem

   
2. **Configure SQL Server**
Edit appsettings.json with your connection string.

3. **Apply migrations**
dotnet ef database update

4. **Run the app**
dotnet run

. **Access in browser:**

https://localhost:5001/books
https://localhost:5001/readers
# library-mvc
