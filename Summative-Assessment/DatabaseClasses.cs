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

    /// <summary>
    /// Prints the books out in a nice to read layout.
    /// </summary>
    /// <param name="books">The list of books to print</param>
    public static void ListBooks(List<Book> books)
    {
        foreach (Book book in books)
        {
            // Makes it so that the dewey decimal number only shows up if the book is non-fiction.
            // Declared out here so that it can be used outside the if else statements.
            string deweyNumber;
            
            string genre;
            if (book.NonFiction == 1)
            {
                // Adds this to the end of the last line so that it looks natural both when it is non-fiction and when it's not.
                deweyNumber = $"\n        Dewey Decimal Number: {book.DeweyNumber}";
                genre = "Non-Fiction";
            } else
            {
                // Means the dewey number won't add anything if the book is fiction.
                deweyNumber = "";
                genre = "Fiction";
            }

            string available;
            if (book.Available == 1)
            {
                available = "Available";
            } else
            {
                available = "Not Available";
            }

            // Index + 1 because it starts at 0.
            string bookPrintStructure = 
            $"""

                {books.IndexOf(book) + 1}) {book.Title}
                    Book Id: {book.Id}
                    Author: {book.AuthorFName} {book.AuthorLName}
                    Genre: {genre}  {deweyNumber}
                    Availablility: {available}
            """;

            Console.WriteLine(bookPrintStructure);
        }
    }

    public static void ListBorrowedBooks(List<Book> books)
    {
        using var db = new LibraryContext();

        foreach (Book book in books)
        {
            // Makes it so that the dewey decimal number only shows up if the book is non-fiction.
            // Declared out here so that it can be used outside the if else statements.
            string deweyNumber;

            BorrowedItem borrowedBook = db.BorrowedItems.Find(book.Id);
            
            string genre;
            if (book.NonFiction == 1)
            {
                // Adds this to the end of the last line so that it looks natural both when it is non-fiction and when it's not.
                deweyNumber = $"\n    Dewey Decimal Number: {book.DeweyNumber}";
                genre = "Non-Fiction";
            } else
            {
                // Means the dewey number won't add anything if the book is fiction.
                deweyNumber = "";
                genre = "Fiction";
            }

            // Index + 1 because it starts at 0.
            string bookPrintStructure = 
            $"""

                {books.IndexOf(book) + 1}) {book.Title}
                    Book Id: {book.Id}
                    Author: {book.AuthorFName} {book.AuthorLName}
                    Genre: {genre}{deweyNumber}
                    Issued: {borrowedBook.DateIssued}
                    Due: {borrowedBook.DateDue}
            """;

            Console.WriteLine(bookPrintStructure);
        }
    }

    /// <summary>
    /// Asks the user for a book id then returns it if it's valid.
    /// </summary>
    public static void ReturnBook()
    {
        const string BookIdPrompt = "Please enter the Id of the book you want to return: ";
        const string InvalidIdMessage = "That isn't a valid book Id. ";
        const string SuccessfulllyReturnedMessage = "Successfully returned book. ";

        // Declares the variable that'll control the loop.
        // Needs to be declared here so that it's in the right scope to affect the loop.
        bool validInput = false;

        // Declares the variable to hold the users input.
        // Its declared here so it can be accessed out of the do while loop.
        // Defaults to 0 so that it guarantees that an int is returned.
        int userInput = 0;

        // Does all the stuff inside the loop before evaluating the loop condition.
        do
        {
            // Tries to convert the user input to int.
            try
            {
                Console.Write(BookIdPrompt);

                // Gets the users input and converts it to int.
                // Throws an error if the users input ism't a number.
                // This causes it to ask again.
                userInput = Convert.ToInt32(Console.ReadLine());

                var db = new LibraryContext();

                // The forced non-null will trigger an error that'll get caught by the catch.
                BorrowedItem borrowedBookToReturn = db.BorrowedItems.Find(userInput)!;
                Book bookToReturn = db.Books.Find(borrowedBookToReturn.Id)!;

                // Makes it so that the dewey decimal number only shows up if the book is non-fiction.
                // Declared out here so that it can be used outside the if else statements.
                string deweyNumber;
                
                string genre;
                if (bookToReturn.NonFiction == 1)
                {
                    // Adds this to the end of the last line so that it looks natural both when it is non-fiction and when it's not.
                    deweyNumber = $"\n    Dewey Decimal Number: {bookToReturn.DeweyNumber}";
                    genre = "Non-Fiction";
                } else
                {
                    // Means the dewey number won't add anything if the book is fiction.
                    deweyNumber = "";
                    genre = "Fiction";
                }
                
                string bookToReturnPrint = 
                $"""
                {bookToReturn.Title}
                    Author: {bookToReturn.AuthorFName} {bookToReturn.AuthorLName}
                    Genre: {genre}{deweyNumber}
                """;

                Console.WriteLine(bookToReturnPrint);

                db.Remove(borrowedBookToReturn);

                bookToReturn.Available = 1;

                db.SaveChanges();

                Console.WriteLine(SuccessfulllyReturnedMessage);

                validInput = true;
            } catch
            {
                Console.WriteLine(InvalidIdMessage);

                // Repeats the loop again so the user has another chance to input a valid option. 
                validInput = false;
            }
        } while (!validInput);
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
    public int Renewed { get; set; }
    // The max number of times a book can be renewed.
    public const int MaxRenews = 2;
    // How long the renewing adds to the loan.
    // In this case it's just the same as the initial borrow length.
    public const int RenewLength = LoanLength;

    // Gets the correct dates when creating the instance.
    public BorrowedItem()
    {
        DateIssued = DateOnly.FromDateTime(DateTime.Now);

        // Adds the loan length to the current date.
        DateDue = DateIssued.AddDays(LoanLength);

        Renewed = 0;
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
    /// Gets the user to select a borrower based on Id.
    /// </summary>
    /// <returns>A Borrower as CurrentBorrower</returns>
    public static CurrentBorrower ChooseBorrower()
    {
        const string BorrowerLoginQuestion = "Please enter your Borrower Id: ";
        const string BorrowerLoginInvalidMessage = "Incorrect Id";

        // Declares the variable that'll control the loop.
        // Needs to be declared here so that it's in the right scope to affect the loop.
        bool validInput;

        // Declares the variable to hold the users input.
        // Its declared here so it can be accessed out of the do while loop.
        // Defaults to 0 so that it guarantees that an int is returned.
        int userInput = 0;

        // Does all the stuff inside the loop before evaluating the loop condition.
        do
        {
            // Tries to convert the user input to int.
            try
            {
                Console.Write(BorrowerLoginQuestion);
                // Gets the users input and converts it to int.
                // Throws an error if the users input ism't a number.
                // This causes it to ask again.
                userInput = Convert.ToInt32(Console.ReadLine());

                CurrentBorrower currentBorrower = new CurrentBorrower(userInput);


                return currentBorrower;
            } catch
            {
                Console.WriteLine(BorrowerLoginInvalidMessage);

                // Repeats the loop again so the user has another chance to input a valid option. 
                validInput = false;
            }
        } while (!validInput);
        
        // Defaults to the first user on the list.
        // Makes the compiler happy.
        return new CurrentBorrower(1);
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

            Book.ListBorrowedBooks(borrowedBooks);
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

    /// <summary>
    /// Pushes the book due date back.
    /// </summary>
    /// <param name="bookId">The Id of the book to renew.</param>
    void RenewBook(int bookId)
    {
        const string InvalidIdMessage = "That isn't a valid Id.";
        const string MaxRenewsMessage = "You've already renewed the max amount of times.";
        string RenewMessage = $"Your book has been renewed for {BorrowedItem.LoanLength} more days.";

        using var db = new LibraryContext();

        try
        {
            // The book Id is being selected from a list of valid books, so it'll always be valid. Though it'll get caught by the catch loop anyway.
            BorrowedItem bookToReturn = db.BorrowedItems.Find(bookId)!;

            if (bookToReturn.Renewed <= BorrowedItem.MaxRenews)
            {
                // Pushes the due date back.
                bookToReturn.DateDue = bookToReturn.DateDue.AddDays(BorrowedItem.RenewLength);
                
                Console.WriteLine(bookToReturn.DateDue);

                // Increases the renew count by one.
                bookToReturn.Renewed++;
                Console.WriteLine(bookToReturn.Renewed);

                db.SaveChanges();

                Console.WriteLine(RenewMessage);
            }
            else
            {
                Console.WriteLine(MaxRenewsMessage);
            }
        }
        catch
        {
            Console.WriteLine(InvalidIdMessage);
        }

        db.SaveChanges();
    }

    void BookOperations()
    {
        const string OperationPrompt = "Enter the number of the book you would like to renew, or 0 to go back to Borrower Menu: ";
        const string InvalidOptionMessage = "That isn't a valid option.";

        /// The number the user has to enter to quit the book operations menu.
        const int quitOption = 0;

        using var db = new LibraryContext();

        // Collects all the books borrowed by the current borrower.
        List<BorrowedItem> borrowedItems = db.BorrowedItems.Where(b => b.BorrowerId == BorrowerId).ToList();

        bool validInput = false;

        // Defaults to -1 because it won't get in the way.
        int userInput = -1;

        while (!validInput)
        {
            // Tries to convert the user input to int.
            try
            {
                Console.Write(OperationPrompt);

                // Gets the users input and converts it to int.
                // Throws an error if the users input ism't a number.
                // This causes it to ask again.
                userInput = Convert.ToInt32(Console.ReadLine());

                BorrowedItem chosenBook = borrowedItems[userInput - 1];

                // The forced non-null will trigger an error that'll get caught by the catch.
                List<Book> book = new List<Book> {db.Books.Find(chosenBook.Id)!};

                Book.ListBorrowedBooks(book);

                // Needs to print out the list of options then take an input.
                string[] bookOptions = ["Renew book", "Cancel"];
                string menuName = "Book Menu";

                int optionChosen = Program.Menu(bookOptions, menuName);

                switch (optionChosen)
                {
                    case 1:
                        int bookToRenew = chosenBook.Id;

                        RenewBook(bookToRenew);
                        break;
                    case 2:
                        return;
                    default:
                        Console.WriteLine(InvalidOptionMessage);
                        break;
                }

                
            }
            catch
            {   
                if (userInput == quitOption)
                {
                    return;
                }
                Console.WriteLine(InvalidOptionMessage);
            }
        }
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
                    BookOperations();
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