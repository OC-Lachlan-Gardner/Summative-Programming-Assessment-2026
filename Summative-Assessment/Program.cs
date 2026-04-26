using System.Dynamic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore.Relational;

public class LibraryContext: DbContext
{
    // Links the table Books with instances of the class Book.
    public DbSet<Book> Books { get; set; }

    public DbSet<Borrower> Borrowers { get; set; }

    public DbSet<BorrowedItem> BorrowedItems { get; set; }

    public void Configure(EntityTypeBuilder<Book> builder)
    {
        
    }

    // Where the database is located.
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Needs to be the whole path.
        optionsBuilder.UseSqlite("Data Source=Library.db");
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
    public float? DeweyNumber { get; set; }

    public int Available { get; set; }

    // Constructor for instances.
    public Book(string title, string authorFName, string authorLName, int nonFiction, float? deweyNumber)
    {
        Title = title;

        AuthorFName = authorFName;

        AuthorLName = authorLName;

        NonFiction = nonFiction;

        DeweyNumber = deweyNumber;
    }
}

public class Borrower(string fName, string lName)
{
    // Makes Id an autonumber
    public int Id { get; set; }
    // First name of the borrower.
    public string FName = fName;

    // Last name of the borrower.
    public string LName = lName;
}

public class BorrowedItem(int bookId, int borrowerId)
{
    // How many weeks the book can be borrowed for.
    public const int WeeksOnLoan = 2;
    
    public const int DaysInAWeek = 7;

    // How long the book can be on loan for in days.
    // The number of weeks * the number of days in a week.
    public const int LoanLength = WeeksOnLoan * DaysInAWeek;

    // The primary key for the book being borrowed.
    public int BookId = bookId;

    // PK of the borrower
    public int BorrowerId = borrowerId;

    // The date the book was issued.
    public DateOnly  DateIssued = DateOnly.FromDateTime(DateTime.Now);

    // The date is due back
    public DateOnly DateDue => DateIssued.AddDays(LoanLength);

    // How many times the book has been renewed.
    // Defaults to 0.
    public int Renewed = 0;

    // An int because sqlite doesn't have booleans.
    // Defaults to 0.
    public int Overdue = 0;

}

class Program
{
    static void Main(string[] args)
    {
        using (var db = new LibraryContext())
        {
            Console.WriteLine("Connection successful.");
            var testBorrower = new Borrower("Lachlan", "Gardner");

            db.Borrowers.Add(testBorrower);
            Console.WriteLine("Adding successful");

            db.SaveChanges();
            Console.WriteLine("Saving successful");
        }
    }
}