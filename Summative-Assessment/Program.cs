using System.Dynamic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

class Program
{
    const string InvalidInputMessage = "That isn't a valid option.";
    const string OptionInputMessage = "Enter an option number: "; 

    /// <summary>
    /// Prints out the welcome message.
    /// </summary>
    static void PrintWelcomeMessage()
    {
        const string Message = "Welcome to The Library.";

        Console.WriteLine(Message);
    }

    /// <summary>
    /// Checks the user's input is within the bounds of the options array.
    /// </summary>
    /// <param name="input">What the user has entered. This is what gets checked.</param>
    /// <param name="options">The options array. Used to get the lenght.</param>
    /// <returns>True if the input is valid.</returns>
    static bool CheckOptionInput(int input, string[] options)
    {
        if (input < options.Length)
        {
            return true;
        } else
        {
            return false;
        }
    }

    /// <summary>
    /// Makes sure the input isn't null.
    /// </summary>
    /// <param name="promptMessage">What to print to the user as a prompt.</param>
    /// <returns>The not null input.</returns>
    static string CheckUserString(string promptMessage)
    {
        string? userInput;

        do
        {
            Console.Write(promptMessage);

            // Gets the users input.
            // If it's null, then it'll throw an error.
            userInput = Console.ReadLine();

            // Prints the invalid message if the user didn't enter anything.            if (userInput == "")
            if (userInput == "")
            {
                Console.WriteLine(InvalidInputMessage);
            }

          // Only loops if the user hasn't input anything.
        } while(userInput == "");

        // By this point userInput has been checked to make sure it isn't null.
        return userInput!;
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

    /// <summary>
    /// Gets the user to select a borrower based on Id.
    /// </summary>
    /// <returns>A Borrower as CurrentBorrower</returns>
    static CurrentBorrower ChooseBorrower()
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
    /// Makes a new Borrower and adds it to the Borrowers table.
    /// It then saves the table.
    /// </summary>
    static void AddNewBorrower()
    {
        const string AddBorrowerFNameMessage = "Please enter your first name: ";
        const string AddBorrowerLNameMessage = "Please enter your last name: ";

        Borrower newBorrower = new Borrower();
        newBorrower.FName = CheckUserString(AddBorrowerFNameMessage);
        newBorrower.LName = CheckUserString(AddBorrowerLNameMessage);

        using var db = new LibraryContext();

        db.Borrowers.Add(newBorrower);
        db.SaveChanges();
    }

    /// <summary>
    /// Prints the books out in a nice to read layout.
    /// </summary>
    /// <param name="books">The list of books to print</param>
    static void ListBooks(List<Book> books)
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
                deweyNumber = $"\n    Dewey Decimal Number: {book.DeweyNumber}";
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
                Author: {book.AuthorFName} {book.AuthorLName}
                Genre: {genre}  {deweyNumber}
                Availablility: {available}
            """;

            Console.WriteLine(bookPrintStructure);
        }
    }

    /// <summary>
    /// Searches through AuthorFName, AuthorLName, and Title in the Books table and lists them.
    /// </summary>
    static void SearchBooks()
    {
        using var db = new LibraryContext();

        const string SearchPrompt = "Enter the keyword you want to search for: ";

        string searchFor = CheckUserString(SearchPrompt);

        var keyword = $"%{searchFor}%";

        List<Book> booksTitles = db.Books.Where(b => EF.Functions.Like(b.Title, keyword)).ToList();
        List<Book> booksAuthorsFName = db.Books.Where(b => EF.Functions.Like(b.AuthorFName, keyword)).ToList();
        List<Book> booksAuthorsLName = db.Books.Where(b => EF.Functions.Like(b.AuthorLName, keyword)).ToList();
        
        List<Book> searchResults = booksTitles.Union(booksAuthorsLName).Union(booksAuthorsFName).ToList();

        ListBooks(searchResults);
    }

    /// <summary>
    /// Asks the user for a book id then returns it if it's valid.
    /// </summary>
    static void ReturnBook()
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

                // Index + 1 because it starts at 0.
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


    /// <summary>
    /// Prints out a list of options as a menu.
    /// </summary>
    /// <param name="options">The list of options to print out.</param>
    /// <returns>The option number of the option they select.</returns>
    static int Menu(string[] options)
    {
        PrintMenuOptions(options);

        // Declares the variable that'll control the loop.
        // Needs to be declared here so that it's in the right scope to affect the loop.
        bool validInput;

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
                validInput = CheckOptionInput(userInput, options);
            } catch
            {
                //! Add a timer then reprint the options again so the user knows what their options are.
                Console.WriteLine(InvalidInputMessage);

                // Repeats the loop again so the user has another chance to input a valid option. 
                validInput = false;
            }
        } while (!validInput);

        return userInput;
    }

    /// <summary>
    /// The switch statement that calls the right functions.
    /// </summary>
    /// <param name="optionChosen">The option the user has chosen.</param>
    static void InitialOptionsMenu(int optionChosen)
    {
        switch (optionChosen)
        {
            case 1:
                ChooseBorrower();
                break;
            case 2:
                AddNewBorrower();
                break;
            case 3:
                SearchBooks();
                break;
            case 4:
                ReturnBook();
                break;
            default:
                break;
        }
    }

    static void Main(string[] args)
    {
        using (var db = new LibraryContext())
        {
            string[] firstMenuOptions = ["Choose Borrower", "Add New Borrower", "Search Books", "Return Book"];
        }

        CurrentBorrower currentBorrower = ChooseBorrower();

        currentBorrower.BorrowBook();
    }
}