using System.Dynamic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

/// <summary>
/// The main program.
/// </summary>
class Program
{
    const string InvalidInputMessage = "That isn't a valid option.";
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
            if (!char.TryParse(userInput, out charInput) || !validInputs.Contains(charInput))
            {
                Console.WriteLine(invalidInputMessage);
            }//!Fix this and add to lowers and test.
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
        // Adds a spacer.
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
        string[] firstMenuOptions = ["Choose Borrower", "Add New Borrower", "Search Books", "Return Book", "Add New Book", "Quit"];

        // Creates a menu with those options.
        return Menu(firstMenuOptions, menuName);
    }

    static void Main(string[] args)
    {
        const string InvalidOptionMessage = "That isn't a valid option.";

        // Which option lines up with switch statement.
        const int BorrowerLoginOption = 1;
        const int AddNewBorrowerOption = 2;
        const int SearchBookOption = 3;
        const int ReturnBookOption = 4;
        const int AddNewBookOption = 5;
        const int QuitOption = 6;

        PrintWelcomeMessage();

        // Says the program is in use.
        bool inUse = true;

        // While the user is using the system.
        while (inUse)
        {
            // What the user has picked with the intial options.
            int optionChosen = MainMenu();

            // Carring out the options.
            switch (optionChosen)
            {
                case BorrowerLoginOption:
                    // Logs into the borrower.
                    CurrentBorrower currentBorrower = CurrentBorrower.ChooseBorrower();
                    currentBorrower.BorrowerOptions();
                    break;
                case AddNewBorrowerOption:
                    // Adds a new borrower to the borrowers table.
                    Borrower.AddNewBorrower();
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