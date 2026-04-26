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
        optionsBuilder.UseSqlite("Data Source=/home/Lachlan/Summative-Programming-Assessment-2026/Summative-Programming-Assessment-2026/Summative-Assessment/Library.db");
    }
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }

    public string AuthorFName { get; set; }

    public string AuthorLName { get; set; }

    //* Defaults to 0 on null.
    //* 0 means it isn't non-fiction.
    public int NonFiction { get; set; } 

    //! Only use if the book is non-fiction. Leave empty otherwise.
    public float DeweyNumber { get; set; }

    public int Available { get; set; }

    // Constructor for instances.
    public Book(string title, string authorFName, string authorLName, int nonFiction, float deweyNumber)
    {
        Title = title;

        AuthorFName = authorFName;

        AuthorLName = authorLName;

        NonFiction = nonFiction;

        DeweyNumber = deweyNumber;

        Available = 0;
    }
}

public class Borrower
{
    // Makes Id an autonumber
    public int Id { get; set; }
    // First name of the borrower.
    public string FName { get; set; }

    // Last name of the borrower.
    public string LName { get; set; }
}

public class BorrowedItem
{
    // How many weeks the book can be borrowed for.
    public const int WeeksOnLoan = 2;
    
    public const int DaysInAWeek = 7;

    // How long the book can be on loan for in days.
    // The number of weeks * the number of days in a week.
    public const int LoanLength = WeeksOnLoan * DaysInAWeek;

    // The primary key for the book being borrowed.
    public int Id { get; set; }

    // PK of the borrower
    public int BorrowerId { get; set; }

    // The date the book was issued.
    public DateOnly DateIssued { get; set; }

    // The date is due back
    public DateOnly DateDue { get; set; }

    // How many times the book has been renewed.
    // Defaults to 0.
    public int Renewed = 0;

    // An int because sqlite doesn't have booleans.
    // Defaults to 0.
    public int Overdue = 0;

    // Gets the correct dates when creating the instance.
    public BorrowedItem()
    {
        DateIssued = DateOnly.FromDateTime(DateTime.Now);

        // Adds the loan length to the current date.
        DateDue = DateIssued.AddDays(LoanLength);
    }
}

class Program
{
    static void welcomeMessage()
        {
            const string Message = 
            """
            Welcome to The Library.
            """;

            Console.WriteLine(Message);
        }

    static void Main(string[] args)
    {
        using (var db = new LibraryContext())
        {
            Console.WriteLine("Connection successful.");

            var testBorrower = new Borrower();
            testBorrower.FName = "Lachlan";
            testBorrower.LName = "Gardner";

            var testBook = new Book("Test title", "Test authorFName", "Test authorLName", 0, 0);

            var testBorrowedItem = new BorrowedItem();
            testBorrowedItem.Id = 2;
            testBorrowedItem.BorrowerId = 1;

            db.Borrowers.Add(testBorrower);
            db.Books.Add(testBook);
            db.BorrowedItems.Add(testBorrowedItem);
            Console.WriteLine("Adding successful");

            db.SaveChanges();
            Console.WriteLine("Saving successful");
        }
    }
}