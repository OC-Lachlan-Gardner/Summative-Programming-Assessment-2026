using System.Dynamic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

/// <summary>
/// The database, represented as class.
/// </summary>
public class LibraryContext: DbContext
{
    // Links the table Books with instances of the class Book.
    public DbSet<Book> Books { get; set; }

    // Links the table Borrowers with instsnces of the class Borrowers.
    public DbSet<Borrower> Borrowers { get; set; }

    // Links the tabke BorrowedItems with instances of the class BorrowedItem.
    public DbSet<BorrowedItem> BorrowedItems { get; set; }

    // Where the database is located.
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Needs to be the whole path.
        optionsBuilder.UseSqlite("Data Source=/home/Lachlan/Summative-Programming-Assessment-2026/Summative-Programming-Assessment-2026/Summative-Assessment/Library.db");
    }
}

/// <summary>
/// Represents individual items in the Books table.
/// </summary>
public class Book
{
    // The PK of the Books table.
    // EFCore automatically marks it as the autonumber because it's called Id.
    public int Id { get; set; }

    // These all match the names of the fields in the Books table.
    // This lets EFCore write the data into the correct places.
    public string Title { get; set; }

    public string AuthorFName { get; set; }

    public string AuthorLName { get; set; }

    //* In sqlite, booleans aren't a thing, so instead it's an int: 1 for true, 0 for false.
    public int NonFiction { get; set; } 

    // Can be left null if it's a fiction book.
    public float? DeweyNumber { get; set; }

    // Bool.
    public int Available { get; set; }

    // What to call books that are non=fiction or fiction.
    const string NonFictionLabel = "Non-Fiction";
    const string FictionLabel = "Fiction";
    const string AvailableLabel = "Available";
    const string UnavailableLabel = "Not Available";

    // What integer counts as true.
    const int trueValue = 1;

    const int AvailableDefault = 1;

    /// <summary>
    /// Create a book instance.
    /// </summary>
    /// <param name="title">Title of the book.</param>
    /// <param name="authorFName">Author's first name.</param>
    /// <param name="authorLName">Author's last name.</param>
    /// <param name="nonFiction">Whether the book is non-fiction. 1 for non-fiction, 0 for fiction.</param>
    /// <param name="deweyNumber">The dewey decimal number of the book if it's non-fiction. If it isn't just leave blank.</param>
    public Book(string title, string authorFName, string authorLName, int nonFiction, float? deweyNumber)
    {
        Title = title;

        AuthorFName = authorFName;

        AuthorLName = authorLName;

        NonFiction = nonFiction;

        // Can be null.
        DeweyNumber = deweyNumber;

        // The book hasn't been borrowed, so it's available.
        Available = AvailableDefault;
    }

    /// <summary>
    /// Prints the books out in a nice to read layout.
    /// </summary>
    /// <param name="books">The list of books to print</param>
    public static void ListBooks(List<Book> books)
    {
        // Iterates through each book in the list to print it out in a nice way.
        foreach (Book book in books)
        {
            // Makes it so that the dewey decimal number only shows up if the book is non-fiction.
            // Declared out here so that it can be used outside the if else statements.
            string deweyNumber;
            
            // Needs to be interpereted from the "bool" the database provides.
            string genre;

            // Checks the books genre.
            if (book.NonFiction == trueValue)
            {
                // Adds this to the end of the last line so that it looks natural both when it is non-fiction and when it's not.
                // It cam't be declared as a constant, I swear it isn't a magic number.
                deweyNumber = $"\n        Dewey Decimal Number: {book.DeweyNumber}";
                genre = NonFictionLabel;
            } else
            {
                // Means the dewey number won't add anything if the book is fiction.
                deweyNumber = "";
                genre = FictionLabel;
            }

            // Needs to interperet the int provided by the database.
            string available;
            if (book.Available == trueValue)
            {
                available = AvailableLabel;
            } else
            {
                available = UnavailableLabel;
            }

            // Index + 1 because it starts at 0.
            // Tbe deweyNumber is empty if the book is fiction.
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

    /// <summary>
    /// Prints a list of the books the borrower has on their account.
    /// </summary>
    /// <param name="books">The books to print.</param>
    public static void ListBorrowedBooks(List<Book> books)
    {
        // Connects to the database.
        using var db = new LibraryContext();

        foreach (Book book in books)
        {
            // Makes it so that the dewey decimal number only shows up if the book is non-fiction.
            // Declared out here so that it can be used outside the if else statements.
            string deweyNumber;

            // Retrieves the book from the BorrowedItems table using the Id of the book it recieves.
            BorrowedItem borrowedBook = db.BorrowedItems.Find(book.Id);
            
            // Turns the int that the book property has and turns it into a string.
            string genre;
            if (book.NonFiction == trueValue)
            {
                // Adds this to the end of the last line so that it looks natural both when it is non-fiction and when it's not.
                deweyNumber = $"\n    Dewey Decimal Number: {book.DeweyNumber}";
                genre = NonFictionLabel;
            } else
            {
                // Means the dewey number won't add anything if the book is fiction.
                deweyNumber = "";
                genre = FictionLabel;
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
        bool validInput;

        // Declares the variable to hold the users input.
        // Its declared here so it can be accessed out of the do while loop.
        int userInput;

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

                // Connects to the database.
                var db = new LibraryContext();

                // The forced non-null will trigger an error that'll get caught by the catch.
                BorrowedItem borrowedBookToReturn = db.BorrowedItems.Find(userInput)!;
                Book bookToReturn = db.Books.Find(borrowedBookToReturn.Id)!;

                // Makes it so that the dewey decimal number only shows up if the book is non-fiction.
                // Declared out here so that it can be used outside the if else statements.
                string deweyNumber;
                
                string genre;
                if (bookToReturn.NonFiction == trueValue)
                {
                    // Adds this to the end of the last line so that it looks natural both when it is non-fiction and when it's not.
                    deweyNumber = $"\n    Dewey Decimal Number: {bookToReturn.DeweyNumber}";
                    genre = NonFictionLabel;
                } else
                {
                    // Means the dewey number won't add anything if the book is fiction.
                    deweyNumber = "";
                    genre = FictionLabel;
                }
                
                string bookToReturnPrint = 
                $"""
                {bookToReturn.Title}
                    Author: {bookToReturn.AuthorFName} {bookToReturn.AuthorLName}
                    Genre: {genre}{deweyNumber}
                """;

                Console.WriteLine(bookToReturnPrint);

                db.Remove(borrowedBookToReturn);

                bookToReturn.Available = trueValue;

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

    /// <summary>
    /// Searches through AuthorFName, AuthorLName, and Title in the Books table and lists them.
    /// </summary>
    public static void SearchBooks()
    {
        // Connects to the database.
        using var db = new LibraryContext();

        // The strings to print to the user on certain interactions.
        // No magic numbers here.
        const string SearchPrompt = "Enter the keyword you want to search for: ";
        const string NoBooksMessage = "\nThere are no books that match the search term.";

        // The minimum count for the list to print anything.
        const int MinCount = 0;

        // What the user is wanting to search for.
        string searchFor = Program.CheckUserString(SearchPrompt);

        // What is avtually passed into the Where function.
        // The % signs on either side means it is looking for that term in any place in the string.
        var keyword = $"%{searchFor}%";

        // Seatches for matches in the titles and author names.
        List<Book> booksTitles = db.Books.Where(b => EF.Functions.Like(b.Title, keyword)).ToList();
        List<Book> booksAuthorsFName = db.Books.Where(b => EF.Functions.Like(b.AuthorFName, keyword)).ToList();
        List<Book> booksAuthorsLName = db.Books.Where(b => EF.Functions.Like(b.AuthorLName, keyword)).ToList();
        
        // Adds all the results together without duplicates.
        List<Book> searchResults = booksTitles.Union(booksAuthorsLName).Union(booksAuthorsFName).ToList();

        // Only prints out the books if there are books to print.
        if (searchResults.Count > MinCount)
        {
            Book.ListBooks(searchResults);
        }
        else
        {
            Console.WriteLine(NoBooksMessage);
        }
    }
}

/// <summary>
/// The object that represents the Borrower table.
/// </summary>
public class Borrower
{
    // Makes Id an autonumber because EFCore automatically knows Id is an autonumber.
    public int Id { get; set; }

    // First name of the borrower.
    public string FName { get; set; }

    // Last name of the borrower.
    public string LName { get; set; }

    /// <summary>
    /// Makes a new Borrower and adds it to the Borrowers table.
    /// It then saves the table.
    /// </summary>
    public static void AddNewBorrower()
    {
        const string AddBorrowerFNameMessage = "Please enter your first name: ";
        const string AddBorrowerLNameMessage = "Please enter your last name: ";

        Borrower newBorrower = new Borrower();
        newBorrower.FName = Program.CheckUserString(AddBorrowerFNameMessage);
        newBorrower.LName = Program.CheckUserString(AddBorrowerLNameMessage);

        using var db = new LibraryContext();

        db.Borrowers.Add(newBorrower);
        db.SaveChanges();

        string successMessage = $"\nCreated new borrower \nName: {newBorrower.FName} {newBorrower.LName} \nId: {newBorrower.Id}";

        Console.WriteLine(successMessage);
    }
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

    /// <summary>
    /// Lists the borrowers books they have on loan.
    /// </summary>
    /// <returns>True if the borrower has books, false if the borrower has no books.</returns>
    public bool ListBorrowerBooks()
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

            return true;
        } else
        {
            Console.WriteLine(NoBooksMessage);

            return false;
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
                    // Checks if the user has books and asks the user to select a book if there are any.
                    if (ListBorrowerBooks())
                    {
                        BookOperations();
                    }
                    break;
                case 2:
                    BorrowBook();
                    break;
                case 3:
                    Book.SearchBooks();
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