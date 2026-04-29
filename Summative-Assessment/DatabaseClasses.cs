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
    const int WeeksOnLoan = 2;
    
    const int DaysInAWeek = 7;

    // How long the book can be on loan for in days.
    // The number of weeks * the number of days in a week.
    const int LoanLength = WeeksOnLoan * DaysInAWeek;

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

    // Gets the correct dates when creating the instance.
    public BorrowedItem()
    {
        DateIssued = DateOnly.FromDateTime(DateTime.Now);

        // Adds the loan length to the current date.
        DateDue = DateIssued.AddDays(LoanLength);
    }
}

class CurrentBorrower
{
    public int BorrowerId { get; set; }

    public string FName { get; set; }

    public string LName { get; set; }

    public CurrentBorrower(int borrowerId)
    {
        BorrowerId = borrowerId;

        using var db = new LibraryContext();

        Borrower currentBorrower = db.Borrowers.Find(BorrowerId);

        FName = currentBorrower.FName;

        LName = currentBorrower.LName;
    }

    /// <summary>
    /// Gets an int from the user then issues it if it's a valid Id.
    /// </summary>
    public void BorrowBook()
    {
        const string InvalidBookIdMessage = "That isn't a valid book Id";
        const string BookIdPrompt = "What is the Id of the book you want to issue: ";

        int bookId;

        try
        {
            Console.Write(BookIdPrompt);

            bookId = Convert.ToInt32(Console.ReadLine());

            using var db = new LibraryContext();

            // Creates an BorrowedItem to add to the BorrowedItews table.
            BorrowedItem borrowedBook = new BorrowedItem();
            borrowedBook.BorrowerId = BorrowerId;
            borrowedBook.Id = bookId;

            // If it causes an error, then it'll get caught by the catch statement.
            Book bookToBorrow = db.Books.Find(bookId)!;
            bookToBorrow.Available = 0;

            db.BorrowedItems.Add(borrowedBook);

            db.SaveChanges();
        } catch
        {
            Console.WriteLine(InvalidBookIdMessage);
        }
    }

    public void ListBorrowerBooks()
    {
        const string NoBooksMessage = "You have no books on loan.";

        using var db = new LibraryContext();

        List<BorrowedItem> borrowedItems = db.BorrowedItems.Where(b => b.BorrowerId == BorrowerId).ToList();

        // If the borrower has books on loan.
        if (borrowedItems.Count() > 0)
        {
            List<Book> borrowedBooks = new List<Book> {};

            foreach (BorrowedItem borrowedItem in borrowedItems)
            {
                borrowedBooks.Add(db.Books.Find(borrowedItem.Id));
            }

            Program.ListBooks(borrowedBooks);
        } else
        {
            Console.WriteLine(NoBooksMessage);
        }
    }

    /// <summary>
    /// Gets the list of books the borrower has on loan,then finds the overdue ones.
    /// Prints out these books.
    /// </summary>
    void BorrowerStats()
    {
        using var db = new LibraryContext();

        List<BorrowedItem> borrowedItems = db.BorrowedItems.Where(b => b.BorrowerId == BorrowerId).ToList();

        int onLoanCount = borrowedItems.Count;

        // The container for the overdue books.
        // Starts at 0 because the number is unknown at that time.
        int overdueBooksCount = 0;

        // Iterates through the borrowed items on the users account and finds all the overdue ones.
        foreach (BorrowedItem borrowedItem in borrowedItems)
        {
            // Checks whether the due date has been past.
            if (borrowedItem.DateDue < DateOnly.FromDateTime(DateTime.Now))
            {
                // Increments by one.
                overdueBooksCount++;
            }
        }

        // The message the user will see.
        // Has a new line at the bottom for readability.
        string borrowerStatsMessage = $"You have {onLoanCount} books on loan. {overdueBooksCount} overdue.";

        Console.WriteLine(borrowerStatsMessage);
    }

    public void BorrowerOptions()
    {
        const string menuName = "Borrower Menu";

        string borrowerLoginMessage = $"\nLogged in as {FName} {LName}";

        Console.WriteLine(borrowerLoginMessage);

        BorrowerStats();

        string[] options = ["List borrowed books", "Issue a book", "Search books", "Logout"];

        bool loggedIn = true;

        while (loggedIn) {
            int optionChosen = Program.Menu(options, menuName);

            switch (optionChosen)
            {
                case 1:
                    ListBorrowerBooks();
                    break;
                case 2:
                    BorrowBook();
                    break;
                case 3:
                    Program.SearchBooks();
                    break;
                default:
                    loggedIn = false;
                    const string logoutMessage = "\nYou have been logged out.";
                    Console.WriteLine(logoutMessage);
                    break;
            }
        }
    }
}
