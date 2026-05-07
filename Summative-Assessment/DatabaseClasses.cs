using System.Dynamic;
using System.Reflection;
using System.Collections;
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
    /// Makes a new book instance and adds it to the books table.
    /// </summary>
    public static void AddNewBook()
    {
        // The max amount of characters the title can be.
        // It's 30000 because the longest book title is 27978 characters long.
        const int MaxTitleLength = 30000;
        // The longest name has 666 characters in it.
        const int MaxNameLength = 666;

        const string TitlePrompt = "Enter the book title: ";
        const string AuthorFNamePrompt = "\nEnter the author's first name: ";
        const string AuthorLNamePrompt = "Enter the author's last name: ";
        const string NonFictionPrompt = "\nIs the book non-fiction. Y/N: ";
        // If the NonFiction answer isn't y or n.
        const string InvalidCharInputPrompt = "That isn't a valid answer.\nPlease enter Y or N.";
        const string DeweyNumberPrompt = "\nWhat is the Dewey Decimal Number: ";
        const int DeweyMin = 0;
        const int DeweyMax = 1000;

        // The values that represent the genres in the database.
        const int NonFiction = 1;
        const int Fiction = 0;

        string InvalidDeweyNumber = $"\nThat isn't a valid Dewey Decimal Number, it must be greater than or equal to {DeweyMin} and less than {DeweyMax}: ";

        // The valid inputs to the Non-Fiction prompt.
        List<char> validCharInputAnswers = new List<char> {'y', 'n'};

        // Which of the valid inputs represents Non-Fiction.
        const int NonFictionIndex = 0;

        // In case the user manages to crash the inputting stage.
        const string InvalidInputErrorMessage = "That isn't valid information, please try again from the main menu.";

        try
        {
            // Declares them here so they can be used outside of any input loops.
            string title;
            string lName;
            string fName;
            // The character that the user inputs to select whether it's non-fiction or not.
            char nonFictionInput;
            // The actual bit that gets passed to the database has to be an int, which represents a bool.
            // Defaults to Fiction.
            int genre = Fiction;
            // Starts as null because it's unknown if the book is non-fiction yet.
            float? deweyNumber = null;

            // Gets the basic book information from the user.
            do
            {
                const string TitleTooLongMessage = "\nTitle is too long";

                // Makes sure the title isn't null.
                title = Program.CheckUserString(TitlePrompt);

                // Checks whether the input title is too long and prints the error message so they can fix it if it is.
                if (title.Count() > MaxTitleLength)
                {
                    Console.WriteLine(TitleTooLongMessage);
                }

            // Repeats if the title is too long.
            } while (title.Count() > MaxTitleLength);

            // Gets the basic book information from the user.
            do
            {
                const string NameTooLongMessage = "\nName is too long";

                // Makes sure the title isn't null.
                fName = Program.CheckUserString(AuthorFNamePrompt);

                // Checks whether the input title is too long and prints the error message so they can fix it if it is.
                if (fName.Count() > MaxNameLength)
                {
                    Console.WriteLine(NameTooLongMessage);
                }

            // Repeats if the name is too long.
            } while (fName.Count() > MaxNameLength);

            // Gets the basic book information from the user.
            do
            {
                const string NameTooLongMessage = "\nName is too long";

                // Makes sure the name isn't null.
                lName = Program.CheckUserString(AuthorLNamePrompt);

                // Checks whether the input name is too long and prints the error message so they can fix it if it is.
                if (lName.Count() > MaxNameLength)
                {
                    Console.WriteLine(NameTooLongMessage);
                }

            // Repeats if the name is too long.
            } while (lName.Count() > MaxNameLength);

            // Asks the user whether the book is non-fiction or not.
            nonFictionInput = Program.CheckUserChar(NonFictionPrompt, InvalidCharInputPrompt, validCharInputAnswers);

            // Continues looping if the users input isn't in the valid answer
            // Makes the user input lower case to make it easier to put in the right answer.
            while (!validCharInputAnswers.Contains(char.ToLower(nonFictionInput)))
            {
                // Asks again if it wasn't calid.
                // This ensures there is a valid answer from the user.
                nonFictionInput = Program.CheckUserChar(InvalidCharInputPrompt, InvalidCharInputPrompt, validCharInputAnswers);
            }
            
            // If the user answers yes to it being a Non-Fiction book.
            if (char.ToLower(nonFictionInput) == validCharInputAnswers[NonFictionIndex])
            {                
                float potentialDeweyNumber;

                string? deweyNumberInput;

                do
                {
                    Console.Write(DeweyNumberPrompt);
                    // What the user enters.
                    // It's nullable because it will be checked later.
                    deweyNumberInput = Console.ReadLine();

                    while (!float.TryParse(deweyNumberInput, out potentialDeweyNumber))
                    {
                        Console.Write(InvalidDeweyNumber);
                        deweyNumberInput = Console.ReadLine();
                    }

                    // Assigns the parsed number to the dewey number.
                    deweyNumber = potentialDeweyNumber;

                    genre = NonFiction;

                    if (deweyNumber < DeweyMin || deweyNumber >= DeweyMax)
                    {
                        Console.WriteLine(InvalidDeweyNumber);
                    }

                // Only continues when the dewey number is neeeded and it's null, or if the dewey number is between 0 and 1000, the dewey decimal range.
                } while (deweyNumber < DeweyMin || deweyNumber >= DeweyMax);
            }

            // Creates an instance using these properties.
            Book bookToAdd = new Book(title, fName, lName, genre, deweyNumber);

            // Makes it so that the dewey decimal number only shows up if the book is non-fiction.
            // Declared out here so that it can be used outside the if else statements.
            string deweyNumberLabel;

            string genreLabel;

            if (genre == trueValue)
            {
                // Adds this to the end of the last line so that it looks natural both when it is non-fiction and when it's not.
                deweyNumberLabel = $"\n    Dewey Decimal Number: {deweyNumber}";
                genreLabel = NonFictionLabel;
            } else
            {
                // Means the dewey number won't add anything if the book is fiction.
                deweyNumberLabel = "";
                genreLabel = FictionLabel;
            }
            
            string bookSummary = 
            $"""

            Title: {title}
                Author: {fName} {lName}
                Genre: {genreLabel}{deweyNumberLabel}

            """;

            //! fix the dewey number printing twice.

            Console.WriteLine(bookSummary);

            const string ConfirmPrompt = "Enter Y to add book, N to cancel: ";

            // Asks the user whether the book is non-fiction or not.
            char confirm = Program.CheckUserChar(ConfirmPrompt, InvalidCharInputPrompt, validCharInputAnswers);

            if (confirm == validCharInputAnswers[NonFictionIndex])
            {
                // Connects to the database so the book can be added to the Books table.
                using var db = new LibraryContext();

                db.Books.Add(bookToAdd);

                // Saves the Books table.
                db.SaveChanges();
                
                // The message to let the user know they've succeddfully added the book.
                const string SuccessMessage = "Successfully added the book.";
                Console.WriteLine(SuccessMessage);
            // By the time the program gets here its already been made sure it's in the valid inputs list.
            } else
            {
                // The message to let the user know they've cancelled the book addition.
                const string CancelMessage = "Cancelled book addition.";
                Console.WriteLine(CancelMessage);
            }            
        }
        catch
        {
            Console.WriteLine(InvalidInputErrorMessage);
        }
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
    /// <returns>true if the book was able to be printed.</returns>
    public static bool ListBorrowedBooks(List<Book> books)
    {
        // Connects to the database.
        using var db = new LibraryContext();

        foreach (Book book in books)
        {
            // Makes it so that the dewey decimal number only shows up if the book is non-fiction.
            // Declared out here so that it can be used outside the if else statements.
            string deweyNumber;

            try
            {
                // Retrieves the book from the BorrowedItems table using the Id of the book it recieves.
                // Will print out the error message if there are no books to print.
                BorrowedItem borrowedBook = db.BorrowedItems.Find(book.Id)!;

                // Turns the int that the book property has and turns it into a string.
                string genre;
                if (book.NonFiction == trueValue)
                {
                    // Adds this to the end of the last line so that it looks natural both when it is non-fiction and when it's not.
                    deweyNumber = $"\n        Dewey Decimal Number: {book.DeweyNumber}";
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
            catch
            {
                const string errorMessage = "There are no books to display. ";

                Console.WriteLine(errorMessage);

                return false;
            }
        }

        // Returns true once it has finished printing.
        return true;
    }

    /// <summary>
    /// Prints a single book, unlike the previous method.`
    /// </summary>
    /// <param name="book">The book to print.</param>
    /// <returns>True if the book was able to be printed.</returns>
    public static bool ListBorrowedBooks(Book book)
    {
        // Connects to the database.
        using var db = new LibraryContext();

        // Makes it so that the dewey decimal number only shows up if the book is non-fiction.
        // Declared out here so that it can be used outside the if else statements.
        string deweyNumber;

        try
        {
            // Retrieves the book from the BorrowedItems table using the Id of the book it recieves.
            BorrowedItem borrowedBook = db.BorrowedItems.Find(book.Id)!;
            
            // Turns the int that the book property has and turns it into a string.
            string genre;
            if (book.NonFiction == trueValue)
            {
                // Adds this to the end of the last line so that it looks natural both when it is non-fiction and when it's not.
                deweyNumber = $"\n        Dewey Decimal Number: {book.DeweyNumber}";
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

                {book.Title}
                    Book Id: {book.Id}
                    Author: {book.AuthorFName} {book.AuthorLName}
                    Genre: {genre}{deweyNumber}
                    Issued: {borrowedBook.DateIssued}
                    Due: {borrowedBook.DateDue}
            """;

            Console.WriteLine(bookPrintStructure);

        } catch
        {
            const string errorMessage = "There was no book to display. ";

            Console.WriteLine(errorMessage);

            return false;
        }

        return false;
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
                
                // Turns the int into an easily readable string.
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

                // Removes the book from the borrowed items table.
                db.Remove(borrowedBookToReturn);

                // Marks the book as available to borrow again.
                bookToReturn.Available = trueValue;

                // Updates the db.
                db.SaveChanges();

                Console.WriteLine(SuccessfulllyReturnedMessage);

                // Tells the loop that the user has entered a valid input so it can stop.
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

        List<Book> orderedSearchResults = searchResults.OrderBy(book => book.AuthorLName).ToList<Book>();

        // Only prints out the books if there are books to print.
        if (orderedSearchResults.Count > MinCount)
        {
            Book.ListBooks(orderedSearchResults);
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
    public Borrower(string fName, string lName)
    {
        FName = fName;

        LName = lName;
    }
    // Makes Id an autonumber because EFCore automatically knows Id is an autonumber.
    public int Id { get; set; }

    // First name of the borrower.
    public string FName { get; private set; }

    // Last name of the borrower.
    public string LName { get; private set; }

    /// <summary>
    /// Makes a new Borrower and adds it to the Borrowers table.
    /// It then saves the table.
    /// </summary>
    public static void AddNewBorrower()
    {
        // Prompts for the user when they're creating the borrower.
        const string AddBorrowerFNameMessage = "Please enter your first name: ";
        const string AddBorrowerLNameMessage = "Please enter your last name: ";

        // Creates a new instance of borrower and then fills it in.
        string fName = Program.CheckUserString(AddBorrowerFNameMessage);
        string lName = Program.CheckUserString(AddBorrowerLNameMessage);
        Borrower newBorrower = new Borrower(fName, lName);

        // Connects to the database so the new borrower can be added.
        using var db = new LibraryContext();

        // Adds the new borrower then saves the database.
        db.Borrowers.Add(newBorrower);
        db.SaveChanges();

        // Lets the user know the creation was successful and provedes some basic information.
        string successMessage = $"\nCreated new borrower \nName: {newBorrower.FName} {newBorrower.LName} \nId: {newBorrower.Id}";

        Console.WriteLine(successMessage);
    }
}

/// <summary>
/// The object that represents the BorrowedItems table.
/// </summary>
public class BorrowedItem
{
    // How many weeks the book can be borrowed for.
    const int WeeksOnLoan = 2;
    // So the weeks on loan can be changed easier than having to add 7.
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

    // The date is due back.
    // It will become DateIssued + LoanLength.
    // I would declare it here, but EFCore can't seem to bind it properly when I do that (probably a skill issue).
    public DateOnly DateDue { get; set; }

    // How many times the book has been renewed.
    // Defaults to 0.
    // I would set it here but EFCore like that.
    public int Renewed { get; set; }
    const int RenewedDefault = 0;
    
    // The max number of times a book can be renewed.
    public const int MaxRenews = 2;
    // How long the renewing adds to the loan.
    // In this case it's just the same as the initial borrow length.
    public const int RenewLength = LoanLength;

    /// <summary>
    /// Gets the correct dates when creating the instance, and adds the loan length to it for date due, then sets the renew count to 0.
    /// </summary>
    public BorrowedItem()
    {
        DateIssued = DateOnly.FromDateTime(DateTime.Now);

        // Adds the loan length to the current date.
        DateDue = DateIssued.AddDays(LoanLength);

        // Sets the renew count to 0.
        Renewed = RenewedDefault;
    }
}

/// <summary>
/// The loggeed in borrower.
/// </summary>
class CurrentBorrower
{
    public int BorrowerId { get; set; }

    public string FName { get; set; }

    public string LName { get; set; }

    // Gets the borrower information based on the Id.
    public CurrentBorrower(int borrowerId)
    {
        BorrowerId = borrowerId;

        // Connects to the databasse so data can be gotten from the Borrowers table.
        using var db = new LibraryContext();

        // Retrieves the borrower with the Id from the table.
        // Forces the find, which will kick it to catch if it doesn't work.
        Borrower currentBorrower = db.Borrowers.Find(BorrowerId)!;

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
        int userInput;

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

                // Creates a new instance of CurrentBorrower to store the current users details in.
                // If it can't find the borrower then it will go to the catch statement.
                CurrentBorrower currentBorrower = new CurrentBorrower(userInput);

                // Exits the method with the newly created currentBorrower.
                return currentBorrower;
            } catch
            {
                // In case the Id doesn't work.
                Console.WriteLine(BorrowerLoginInvalidMessage);

                // Repeats the loop again so the user has another chance to input a valid option. 
                validInput = false;
            }
        // Keeps going until the user enters a correct Id.
        } while (!validInput);
        
        // Defaults to the first user on the list.
        // Makes the compiler happy, can't actually happen since the loop won't ecit until a valid input is entered, which would lead to a currentBorrower being returned.
        return new CurrentBorrower(1);
    }

    /// <summary>
    /// Gets an int from the user then issues it if it's a valid Id.
    /// </summary>
    public void BorrowBook()
    {
        const string InvalidBookIdMessage = "\nThat isn't a valid book Id";
        const string BookIdPrompt = "What is the Id of the book you want to issue: ";


        try
        {
            Console.Write(BookIdPrompt);

            int bookId = Convert.ToInt32(Console.ReadLine());

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

            // The forced non-null will trigger an error that'll get caught by the catch.
            Book book = db.Books.Find(borrowedBook.Id)!;

            // Prints the book so the user can see what they issued.
            Book.ListBorrowedBooks(book);
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
        const int MinCount = 0;

        // Connects to the BorrowedItems table so it can get the books on loan.
        using var db = new LibraryContext();

        try
        {
            // Finds all the books that were borrowed by the person with the currentBorrowers Id.
            List<BorrowedItem> borrowedItems = db.BorrowedItems.Where(b => b.BorrowerId == BorrowerId).ToList();

            // If the borrower has books on loan.
            if (borrowedItems.Count() > MinCount)
            {
                // Makes a new list to store the books in once they've been found.
                List<Book> borrowedBooks = new List<Book> {};

                // Orders the list by the date due.
                List<BorrowedItem> orderedBooks = borrowedItems.OrderBy(book => book.DateDue).ToList();

                // Goes through the borrowed books to find the full information.
                foreach (BorrowedItem borrowedItem in orderedBooks)
                {
                    try
                    {
                        // Adds each book to the list.
                        // It won't be null because the books can't be issued without existing, esoecially since there isn't a way to remove Books entries.
                        // Will cause an error if there isn't a book to find, which will go to the catch statement.
                        borrowedBooks.Add(db.Books.Find(borrowedItem.Id)!);
                    }
                    catch
                    {
                        Console.WriteLine(NoBooksMessage);

                        return false;
                    }
                    
                }


                // Prints out the list.
                if (Book.ListBorrowedBooks(borrowedBooks))
                {
                    return true;
                }
                else
                {
                    // Moves to the catch part and uses the no books message as the error message
                    throw new Exception(NoBooksMessage);
                }
            } else
            {
                Console.WriteLine(NoBooksMessage);

                // Says there were no books to list.
                return false;
            }
        } catch (Exception e)
        {
            Console.WriteLine(e);

            return false;
        }
    }

    /// <summary>
    /// Gets the list of books the borrower has on loan,then finds the overdue ones.
    /// Prints out these books.
    /// </summary>
    void BorrowerStats()
    {
        // Connects to the BorrowedItems to get the stats of the borrower.
        using var db = new LibraryContext();

        // Gets the borrowedItems that were borrowed by the same borrower.
        List<BorrowedItem> borrowedItems = db.BorrowedItems.Where(b => b.BorrowerId == BorrowerId).ToList();

        // Finds how many books in the list.
        int onLoanCount = borrowedItems.Count;

        const int OverdueBooksCountDefault = 0;

        // The container for the overdue books.
        // Starts at 0 because the number is unknown at that time.
        int overdueBooksCount = OverdueBooksCountDefault;

        // Iterates through the borrowed items on the users account and finds all the overdue ones.
        foreach (BorrowedItem borrowedItem in borrowedItems)
        {
            // Checks whether the due date of the borrowed item has been past.
            if (borrowedItem.DateDue < DateOnly.FromDateTime(DateTime.Now))
            {
                // Increments by one.
                overdueBooksCount++;
            }
        }

        // The message the user will see.
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

        // Connects to the database to find the borrowed items with that book Id.
        using var db = new LibraryContext();

        try
        {
            // The book Id is being selected from a list of valid books, so it'll always be valid. Though it'll get caught by the catch loop anyway.
            BorrowedItem bookToReturn = db.BorrowedItems.Find(bookId)!;

            // Only renews if the user hasn't renewed too many times already.
            if (bookToReturn.Renewed <= BorrowedItem.MaxRenews)
            {
                // Pushes the due date back.
                bookToReturn.DateDue = bookToReturn.DateDue.AddDays(BorrowedItem.RenewLength);

                // Increases the renew count by one.
                bookToReturn.Renewed++;

                // Saves the changes to the database.
                db.SaveChanges();

                // Tells the user they've successfully renewed and says how long.
                Console.WriteLine(RenewMessage);
            }
            else
            {
                // Tells the user they've reached the max renew count and can't renew anymore.
                // Means they won't be confused when they can't renew.
                Console.WriteLine(MaxRenewsMessage);
            }
        }
        catch
        {
            Console.WriteLine(InvalidIdMessage);
        }

        // Saves any changes made to the tables.
        db.SaveChanges();
    }

    /// <summary>
    /// What actions the user can take after listing the books.
    /// </summary>
    void BookOperations()
    {
        const string OperationPrompt = "\nEnter the number of the book you would like to renew, or 0 to go back to Borrower Menu: ";
        const string InvalidOptionMessage = "That isn't a valid option.";

        /// The number the user has to enter to quit the book operations menu.
        const int quitOption = 0;

        // Connects to the database to find the borrowed items.
        using var db = new LibraryContext();

        // Collects all the books borrowed by the current borrower.
        List<BorrowedItem> borrowedItems = db.BorrowedItems.Where(b => b.BorrowerId == BorrowerId).ToList();

        // Allows the loop to start.
        // It's false because there isn't any input right now.
        bool validInput = false;

        // Defaults to -1 because it will cause an error if it isn't changed.
        // This will make the loop repeat so the user can enter a proper number.
        const int UserInputDefault = -1;
        int userInput = UserInputDefault;

        // Loops until the user enters a valid input.
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

                // userInput minus 1 so it matches up to the list index, which starts at 0.
                BorrowedItem chosenBook = borrowedItems[userInput - 1];

                // Gets all the information about the book after getting its Id from the BorrowedItems table.
                // Getting it this way means that it can't accidentally get a book that hasn't been borrowed.
                // The forced non-null will trigger an error that'll get caught by the catch, causing the loop to go again.
                Book book = db.Books.Find(chosenBook.Id)!;

                // Prints the book in a nice way.
                Book.ListBorrowedBooks(book);

                // Needs to print out the list of options then take an input.
                string[] bookOptions = ["Renew book", "Cancel"];
                const string menuName = "Book Menu";

                // Makes a menu with the options from the list.
                int optionChosen = Program.Menu(bookOptions, menuName);

                // What to do based on the option chosen.
                switch (optionChosen)
                {
                    case 1:
                        // The user has chosen to renew, so it gets the Id from the book they initially chose and passes it to the Renew function.
                        int bookToRenew = chosenBook.Id;

                        // Renews the book if it can be renewed.
                        RenewBook(bookToRenew);
                        break;
                    case 2:
                        // Exits the book operations function and returns to the borrowers menu.
                        return;
                    default:
                        Console.WriteLine(InvalidOptionMessage);
                        // Exits back into the loop.
                        break;
                }                
            }
            catch
            {   
                // If the user has chosen to quit the Book Operations menu.
                // Goes back to the Borrowers menu.
                if (userInput == quitOption)
                {
                    return;
                }
                // If the input wasn't a book number or the quit option.
                // It's not an else because if the if statement is true then it'll quit the function without the chance to get to this place anyway.
                Console.WriteLine(InvalidOptionMessage);
            }
        }
    }

    /// <summary>
    /// What options the borrower has.
    /// Takes their input and carries out the option selected, if its valid.
    /// </summary>
    public void BorrowerOptions()
    {
        // What to print at the top of the menu.
        // Gives the user an idea of what the menu is about.
        const string menuName = "Borrower Menu";

        // Small informational message to feel responsive to the user.
        string borrowerLoginMessage = $"\nLogged in as {FName} {LName}";

        Console.WriteLine(borrowerLoginMessage);

        // Shows the borrower a small view of their current loans.
        BorrowerStats();

        // What options are available to the user in this menu.
        string[] options = ["List borrowed books", "Issue a book", "Search books", "Logout"];

        // Controls the loop.
        bool loggedIn = true;

        while (loggedIn) {
            // Prints a menu and stores the option they chose.
            int optionChosen = Program.Menu(options, menuName);

            // Acting on their choice.
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
                    // So they can search for books without having to exit the borrower menu.
                    // In case they quickly want to search for something, maybe the what book in the series it is.
                    Book.SearchBooks();
                    break;
                default:
                    // Exits the loop.
                    // This will kick it back into the main menu.
                    loggedIn = false;
                    const string logoutMessage = "\nYou have been logged out.";
                    Console.WriteLine(logoutMessage);
                    break;
            }
        }
    }
}