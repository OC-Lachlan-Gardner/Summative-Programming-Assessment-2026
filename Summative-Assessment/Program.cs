using System.Dynamic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

public class LibraryContext: DbContext
{
    // Links the table Books with instances of the class Book.
    public DbSet<Book> Books { get; set; }

    public DbSet<Borrower> Borrowers { get; set; }

    public DbSet<BorrowedItem> BorrowedItems { get; set; }

    // Where the database is located.
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Needs to be the whole path.
        optionsBuilder.UseSqlite("Data Source=Library.db");
    }
}

public class Book
{
    required public string Title { get; set; }

    required public string AuthorFName { get; set; }

    public string AuthorLName { get; set; }

    //* Defaults to 0 on null.
    //* 0 means it isn't non-fiction.
    public int NonFiction { get; set; } 

    //! Only use if the book is non-fiction. Leave empty otherwise.
    public float DeweyNumber { get; set; }

    // Constructor for instances.
    public Book(string title, string authorFName, string authorLName, int nonFiction, int deweyNumber)
    {
        Title = title;

        AuthorFName = authorFName;

        AuthorLName = authorLName;

        NonFiction = nonFiction;

        DeweyNumber = deweyNumber;
    }
}

public class Borrower
{
    // First name of the borrower.
    required public string FName { get; set; }

    // Last name of the borrower.
    required public string LName { get; set; }

    // Constructor for Borrowers to make adding new ones more readable.
    public Borrower(string firstName, string lastName)
    {
        // Assigning the data passed into the constructor.
        FName = firstName;

        LName = lastName;
    }
}

public class BorrowedItem
{
    // The primary key for the book being borrowed.
    required public int BookID { get; set; }

    // PK of the borrower
    required public int BorrowerID { get; set; }

    // The date the book was issued.
    required public DateTime DateIssued { get; set; }

    // The date is due back
    required public DateTime DateDue { get; set; }

    // How many times the book has been renewed.
    // Defaults to 0 on null.
    public int Renewed { get; set; }
}