# library

## LibraryApp

An **ASP.NET Core MVC application** for managing books and readers.  
It provides CRUD operations, search, borrowing/returning books. 

---
By default, the application starts with 10 books and 10 readers. If you want to add more (up to 100k),
you can do a bulk insert by copying the content of Database/DatabaseSeed100k.SQL and execute it against your SQL server instance.

##  Tech Stack
- **DOT .NET:** 9.0
--> winget install Microsoft.DotNet.SDK.9

- **Backend:** ASP.NET Core MVC (C#)

- **Database:** SQL Server + Entity Framework Core
	--> winget install --id=Microsoft.SQLServer.2022.Express  -e

## ⚙️ Installation & Setup
**Before you start, make sure you have --.NET SDK 9.0-- and --SQL Server-- installed.**

1. **Clone repository**
   git clone https://github.com/arion-marius/LibraryApp.git
   cd LibraryApp

2. **Configure SQL Server**
Edit appsettings.json with your connection string.
ex: "Server=.\\SQLEXPRESS;Database=Library;Trusted_Connection=True;TrustServerCertificate=True;"

3. **Apply migrations**
dotnet ef database update

4. **Run the app**
dotnet run

**Access in browser:**

https://localhost:5001
