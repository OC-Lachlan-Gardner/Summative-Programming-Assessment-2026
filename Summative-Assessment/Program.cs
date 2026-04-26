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
    required public string FName { get; set; }

    // Last name of the borrower.
    required public string LName { get; set; }
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
    const string InvalidInputMessage = "That isn't a valid option.";
    const string OptionInputMessage = "Enter an option number: "; 

    static void PrintWelcomeMessage()
    {
        const string Message = "Welcome to The Library.";

        Console.WriteLine(Message);
    }

    static bool CheckOptionInput(int input, string[] options)
    {
        if (input < options.Length)
        {
            return false;
        } else
        {
            return true;
        }
    }

    /// <summary>
    /// Prints each of the options and its option number.
    /// </summary>
    /// <returns>The option number.</returns>
    static void PrintMenuOptions(string[] options)
    {
        // Iterates through the options, printing each option and its option number.
        foreach (string option in options)
        {
            // The message to print.
            // Index + 1 is to offset the index starting from 0.
            string message = $"{options.IndexOf(option) + 1}) {option}.";

            Console.WriteLine(message);
        }
    }

    static int Menu(string[] options)
    {
        PrintMenuOptions(options);

        // Declares the variable that'll control the loop.
        // Needs to be declared here so that it's in the right scope to affect the loop.
        bool shouldRepeat;

        // Declares the variable to hold the users input.
        // Its declared here so it can be accessed out of the do while loop.
        // Defaults to 0 so that ir guarantees that an int is returned.
        int userInput = 0;

        // Does all the stuff inside the loop before evaluating the loop condition.
        do
        {
            // Tries to convert the user input to int.
            try
            {
                Console.Write(OptionInputMessage);
                // Gets the users input and converts it to int.
                // Throws an error if the users input is a number.
                // This causes it to ask again.
                userInput = Convert.ToInt32(Console.ReadLine());

                // Checks the input maps to a valid option.
                shouldRepeat = CheckOptionInput(userInput, options);
            } catch
            {
                Console.WriteLine(InvalidInputMessage);

                // Repeats the loop again so the user has another chance to input a valid option. 
                shouldRepeat = true;
            }
        } while (shouldRepeat);

        return userInput;
    }

    static void Main(string[] args)
    {
        using (var db = new LibraryContext())
        {
            string[] firstMenuOptions = ["Choose Borrower", "Add New Borrower", "Search Books", "Return Book"];

            Console.WriteLine(Menu(firstMenuOptions));

            Console.WriteLine("Connection successful.");

            PrintWelcomeMessage();
        }
    }
}