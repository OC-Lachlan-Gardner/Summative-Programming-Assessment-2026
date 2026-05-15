using System.Dynamic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

/// <summary>
/// The main program.
/// </summary>
class Program
{
    // What to print to the user when they enter an invalid input.
    const string InvalidInputMessage = "That isn't a valid input.";

    // The prompt for the user to enter a number.
    const string OptionInputMessage = "Enter an option number: "; 

    /// <summary>
    /// Prints out the welcome message.
    /// </summary>
    static void PrintWelcomeMessage()
    {
        // Adds a newline to the start of the message to add some clarity between the parts of the program.
        const string Message = "\nWelcome to The Library.";

        Console.WriteLine(Message);
    }

    /// <summary>
    /// Checks the user's input is within the bounds of the options array.
    /// </summary>
    /// <param name="input">What the user has entered. This is what gets checked.</param>
    /// <param name="options">The options array. Used to get the length.</param>
    /// <returns>True if the input is valid. False if not.</returns>
    static bool CheckOptionInput(int input, string[] options)
    {
        const string InvalidInputMessage = "That isn't a valid option.";
        // The lowest number of the options while still being valid.
        const int MinInput = 1;

        // Check whether the option is within the valid range.
        if (input <= options.Count() && input >= MinInput)
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
    public static string CheckUserString(string promptMessage)
    {
        // Puts the variable in the right scope so it can be accessed outside the loop.
        // It's nullable because the result of user input is unknown if it is null.
        string? userInput;

        do
        {
            Console.Write(promptMessage);

            // Gets the users input.
            // If it's null, then it'll get caught by the if statement.
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
    /// Makes sure the input isn't null.
    /// </summary>
    /// <param name="promptMessage">What to print to the user as a prompt.</param>
    /// <returns>The not null input.</returns>
    public static char CheckUserChar(string promptMessage, string invalidInputMessage, List<char> validInputs)
    {        
        // Puts the variable in the right scope so it can be accessed outside the loop.
        string? userInput;

        char charInput;

        do
        {
            Console.Write(promptMessage);

            // Gets the users input.
            // If it's null, then it'll get caught by the if statement.
            userInput = Console.ReadLine();

            // Prints the invalid message if the user didn't enter anything.            if (userInput == "")
            if (!char.TryParse(userInput, out charInput) || !validInputs.Contains(char.ToLower(charInput)))
            {
                Console.WriteLine(invalidInputMessage);
            }
        // Only loops if the user hasn't input anything or it can't be converted to char.
        } while(!char.TryParse(userInput, out charInput) || !validInputs.Contains(char.ToLower(charInput)));

        // By this point userInput has been checked to make sure it isn't null.
        return charInput;
    }

    /// <summary>
    /// Prints each of the options and its option number.
    /// </summary>
    /// <returns>The option number the user has input.</returns>
    public static void PrintMenuOptions(string[] options)
    {
        // Iterates through the options, printing each option and its option number.
        foreach (string option in options)
        {
            // The message to print.
            // Index + 1 is to offset the index starting from 0.
            // The weird spacing is to make it look nice in the terminal.
            string message = $"    {options.IndexOf(option) + 1}) {option}.";

            Console.WriteLine(message);
        }
    }


    /// <summary>
    /// Prints out a list of options as a menu.
    /// </summary>
    /// <param name="options">The list of options to print out.</param>
    /// <returns>The option number of the option they select.</returns>
    public static int Menu(string[] options, string menuName)
    {
        const char MenuCharacter = ':';
        // Adds a spacer.
        Console.WriteLine();
        // Adds the colon on the end to make it clear the user is expected to input something.
        Console.WriteLine(menuName + MenuCharacter);

        // Print the options to the user.
        PrintMenuOptions(options);

        // Declares the variable that'll control the loop.
        // Needs to be declared here so that it's in the right scope to affect the loop.
        bool validInput;

        // The int to return if the userInput isn't touched, somehow (It makes the compiler happy).
        const int InputDefault = 0;

        // Declares the variable to hold the users input.
        // Its declared here so it can be accessed out of the do while loop.
        // Defaults to 0 so that ir guarantees that an int is returned.
        int userInput = InputDefault;

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
                Console.WriteLine(InvalidInputMessage);

                // Repeats the loop again so the user has another chance to input a valid option. 
                validInput = false;
            }
        } while (!validInput);

        Console.WriteLine();

        return userInput;
    }

    /// <summary>
    /// Checks whether the database can be written to.
    /// </summary>
    /// <returns>True if the database can be connected to and saved. False if not.</returns>
    public static bool CheckDatabase()
    {
        try
        {
            var db = new LibraryContext();

            const int TestNumber = 1;
            // Tries to find data in the database, crashes if it can't be found.
            db.Books.Find(TestNumber);

            return true;
        }
        catch
        {
            // Tells the user what's wrong.
            const string DatabaseFailedMessage = "The database couldn't be connected to. \nPlease fix this and try again.";
            Console.WriteLine(DatabaseFailedMessage);

            return false;
        }
    }

    /// <summary>
    /// The main menu that greets the user at the start.
    /// </summary>
    /// <returns>The option they choose.</returns>
    public static int MainMenu()
    {
        const string menuName = "Main Menu";

        // The list of options for the user to choose from at the start of the program.
        string[] firstMenuOptions = ["List All Borrowers", "Choose Borrower", "Add New Borrower", "Remove A Borrower", "List All Books", "Search Books", "Return Book", "Add New Book", "Remove A Book", "Quit"];

        // Creates a menu with those options.
        return Menu(firstMenuOptions, menuName);
    }

    static void Main(string[] args)
    {
        if (!CheckDatabase())
        {
            return;
        }
        const string InvalidOptionMessage = "That isn't a valid option.";

        // Which option lines up with switch statement.
        const int ListAllBorrowersOption = 1;
        const int BorrowerLoginOption = 2;
        const int AddNewBorrowerOption = 3;
        const int RemoveBorrowerOption = 4;
        const int ListAllBooksOption = 5;
        const int SearchBookOption = 6;
        const int ReturnBookOption = 7;
        const int AddNewBookOption = 8;
        const int RemoveBookOption = 9;
        const int QuitOption = 10;

        PrintWelcomeMessage();

        // Says the program is in use.
        bool inUse = true;

        // While the user is using the system.
        while (inUse)
        {
            // What the user has picked with the intial options.
            int optionChosen = MainMenu();

            // Carring out the options the user has entered.
            switch (optionChosen)
            {
                //! This doesn't work.
                //! And list borrowers needs commenting.
                case ListAllBorrowersOption:
                    Borrower.ListAllBorrowers();
                    break;
                case BorrowerLoginOption:
                    // Logs into the borrower.
                    CurrentBorrower currentBorrower = CurrentBorrower.ChooseBorrower();
                    currentBorrower.BorrowerOptions();
                    break;
                case AddNewBorrowerOption:
                    // Adds a new borrower to the borrowers table.
                    Borrower.AddNewBorrower();
                    break;
                case RemoveBorrowerOption:
                    Borrower.RemoveBorrower();
                    break;
                case ListAllBooksOption:
                    // Lists all the books in the database.
                    // Means the user can find the book Id.
                    Book.ListAllBooks();
                    break;
                case SearchBookOption:
                    // Searches through the Books table to find matching books.
                    Book.SearchBooks();
                    break;
                case ReturnBookOption:
                    // Returns (as in returning a loan to the library) a book based on the Id.
                    Book.ReturnBook();
                    break;
                case AddNewBookOption:
                    Book.AddNewBook();
                    break;
                case RemoveBookOption:
                    Book.RemoveBook();
                    break;
                case QuitOption:
                    // Exits the loop.
                    inUse = false;
                    break;
                default:
                    // In case the user inputs 0.
                    Console.WriteLine(InvalidOptionMessage);
                    break;
            }
        }
    }
}