# library

## Library Management System

An **ASP.NET Core MVC application** for managing books and readers.  
It provides CRUD operations, search, pagination, borrowing/returning books with validations who are using custom exceptions.  

---

## Features

### Books (10 books default)
- Add, edit, delete books
- Pagination & search
- Validations & exceptions:
  - Title required
  - Author required & max length
  - Prevent duplicates
  - Prevent deletion of borrowed books

### Readers (10 readers default)
- Add, edit, delete readers
- Email validation (required, unique, max length)
- Prevent deletion if reader has borrowed books
- Show borrowed books with history & return dates
- Track overdue readers (borrowed > 1 month)



### Bulk insert
 **100k books and readers** 
 via SQL seed script(if you want to use it, go to Database -> DatabaseSeed100k.SQL, copy, open SQL and paste it in your SQLQuery.
 If you want to delete all data for the moment, open SQL and write in SQLQuery these 2 commands: DELETE FROM Readers WHERE Id > 0
          DELETE FROM Books WHERE Id > 0), if you close the application, the database returns to its default form.



### Borrow & Return
- Borrow books with validations:
  - Max 5 books per reader
  - No duplicate borrowing
  - Prevent borrowing if out of stock
- Return books and auto-update:
  - Book stock increases
  - Reader’s borrowed count decreases

###  Notifications
- Uses `TempData` for **success/warning alerts**

---

##  Tech Stack
- **Backend:** ASP.NET Core MVC (C#)
- **Database:** SQL Server + Entity Framework Core
- **UI:** Razor Views + Bootstrap
- **Pagination:** X.PagedList
- **Serialization:** System.Text.Json

## ⚙️ Installation & Setup

1. **Clone repository**
   ```sh
   git clone https://github.com/your-username/LibraryManagementSystem.git
   cd LibraryManagementSystem

   
2. **Configure SQL Server**
Edit appsettings.json with your connection string.

3. **Apply migrations**
dotnet ef database update

4. **(Optional) Seed 100k books/readers
Run the provided DatabaseSeed100k.sql script:

await _dbContext.Database.ExecuteSqlRawAsync("DatabaseSeed100k.sql");

5. **Run the app**
dotnet run

6. **Access in browser:**

https://localhost:5001/books
https://localhost:5001/readers
# library-mvc
