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
        // Adds a newline to the end of the message to add some clarity between the parts of the program.
        const string Message = "\nWelcome to The Library.";

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
        const string InvalidInputMessage = "That isn't a valid option.";

        if (input <= options.Count())
        {
            return true;
        } else
        {
            Console.WriteLine(InvalidInputMessage);
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
    public static void PrintMenuOptions(string[] options)
    {
        // Iterates through the options, printing each option and its option number.
        foreach (string option in options)
        {
            // The message to print.
            // Index + 1 is to offset the index starting from 0.
            string message = $"    {options.IndexOf(option) + 1}) {option}.";

            Console.WriteLine(message);
        }
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
    /// Searches through AuthorFName, AuthorLName, and Title in the Books table and lists them.
    /// </summary>
    public static void SearchBooks()
    {
        using var db = new LibraryContext();

        const string SearchPrompt = "Enter the keyword you want to search for: ";
        const string NoBooksMessage = "There are no books that match the search term.";

        string searchFor = CheckUserString(SearchPrompt);

        var keyword = $"%{searchFor}%";

        List<Book> booksTitles = db.Books.Where(b => EF.Functions.Like(b.Title, keyword)).ToList();
        List<Book> booksAuthorsFName = db.Books.Where(b => EF.Functions.Like(b.AuthorFName, keyword)).ToList();
        List<Book> booksAuthorsLName = db.Books.Where(b => EF.Functions.Like(b.AuthorLName, keyword)).ToList();
        
        List<Book> searchResults = booksTitles.Union(booksAuthorsLName).Union(booksAuthorsFName).ToList();

        if (searchResults.Count > 0)
        {
            Book.ListBooks(searchResults);
        }
        else
        {
            Console.WriteLine(NoBooksMessage);
        }
    }

    /// <summary>
    /// Prints out a list of options as a menu.
    /// </summary>
    /// <param name="options">The list of options to print out.</param>
    /// <returns>The option number of the option they select.</returns>
    public static int Menu(string[] options, string menuName)
    {
        // Adds a space.
        Console.WriteLine();
        Console.WriteLine(menuName + ":");

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

        Console.WriteLine();

        return userInput;
    }

    /// <summary>
    /// The main menu that greets the user at the start.
    /// </summary>
    /// <returns>The option they choose.</returns>
    public static int MainMenu()
    {
        const string menuName = "Main Menu";
        // The list of options for the user to choose from at the start of the program.
        string[] firstMenuOptions = ["Choose Borrower", "Add New Borrower", "Search Books", "Return Book", "Quit"];

        // Creates a menu with those options.
        return Menu(firstMenuOptions, menuName);
    }

    static void Main(string[] args)
    {
        PrintWelcomeMessage();

        // Says the program is in use.
        bool inUse = true;

        while (inUse)
        {
            // What the user has picked with the intial options.
            int optionChosen = MainMenu();

            // Carring out the options.
            switch (optionChosen)
            {
                case 1:
                    // Logs into the borrower.
                    CurrentBorrower currentBorrower = CurrentBorrower.ChooseBorrower();
                    currentBorrower.BorrowerOptions();
                    break;
                case 2:
                    // Adds a new borrower to the borrowers table.
                    AddNewBorrower();
                    break;
                case 3:
                    // Searches through the Books table to find matching books.
                    SearchBooks();
                    break;
                case 4:
                    // Returns (as in returning a loan) a book based on the Id.
                    Book.ReturnBook();
                    break;
                default:
                    // Exits the loop.
                    inUse = false;
                    break;
            }
        }
        
    }
}