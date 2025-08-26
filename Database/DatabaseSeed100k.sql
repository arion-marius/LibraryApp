
DECLARE @i INT = 1;
WHILE @i < 100000
BEGIN
INSERT INTO Books(Title, Author, Stock)
VALUES('Book_' + CONVERT(NVARCHAR, @i), 'Author_' + CONVERT(NVARCHAR, @i), 99)
INSERT INTO Readers (Name, Email, BooksBorrowed)
VALUES ('Reader_' + CONVERT(NVARCHAR, @i), 'reader_' + CONVERT(NVARCHAR, @i) + '@gmail.com', 0)
SET @i=@i+1;
END