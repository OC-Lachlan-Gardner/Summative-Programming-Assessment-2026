using System.Dynamic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

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

    static CurrentBorrower ChooseBorrower()
    {
        const string BorrowerLoginQuestion = "Please enter your Borrower Id: ";
        const string BorrowerLoginInvalidMessage = "Incorrect Id";
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
                Console.Write(BorrowerLoginQuestion);
                // Gets the users input and converts it to int.
                // Throws an error if the users input is a number.
                // This causes it to ask again.
                userInput = Convert.ToInt32(Console.ReadLine());

                CurrentBorrower currentBorrower = new CurrentBorrower(userInput);


                return currentBorrower;
            } catch
            {
                Console.WriteLine(BorrowerLoginInvalidMessage);

                // Repeats the loop again so the user has another chance to input a valid option. 
                shouldRepeat = true;
            }
        } while (shouldRepeat);
        
        // Defaults to the first user on the list.
        // Makes the compiler happy.
        return new CurrentBorrower(1);
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

    static void InitialOptionsMenu(int optionChosen)
    {
        // switch (optionChosen)
        // {
        //     case 1:
        //         ChooseBorrower();
        //         break;
        //     case 2:
        //         AddNewBorrower();
        //         break;
        //     case 3:
        //         SearchBooks();
        //         break;
        //     case 4:
        //         ReturnBook();
        //         break;
        //     default:
        //         break;
        // }
    }

    static void Main(string[] args)
    {
        using (var db = new LibraryContext())
        {
            string[] firstMenuOptions = ["Choose Borrower", "Add New Borrower", "Search Books", "Return Book"];

        }

        CurrentBorrower TestBorrower = ChooseBorrower();

        Console.WriteLine(TestBorrower.FName);
    }
}