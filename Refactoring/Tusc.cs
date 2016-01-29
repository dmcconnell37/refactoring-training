using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactoring
{
    public class Tusc
    {
        private static List<User> users;
        private static List<Product> products;
        private static int numberOfUsers;
        private static int numberOfProducts;

        public static void Start(List<User> usrs, List<Product> prods)
        {
            // This feels a bit like a global variable declaration/use, even though it is within the class.
            // But, I can live with it for the first attempt. :)
            users = usrs;
            products = prods;
            numberOfUsers = users.Count();
            numberOfProducts = products.Count();
            
            // Write welcome message
            Console.WriteLine("Welcome to TUSC");
            Console.WriteLine("---------------");

            // Login
            Login:
            
            // Prompt for user input
            WriteBlankLineAndMessage("Enter Username:");
            string enteredUserName = Console.ReadLine();

            // Validate Username
            if (!string.IsNullOrEmpty(enteredUserName))
            {
                if (IsValidUser (enteredUserName))
                {
                    WriteMessage("Enter Password:");
                    string enteredPassword = Console.ReadLine();

                    if (IsValidPassword(enteredUserName, enteredPassword))
                    {
            
                        // Show welcome message
                        Console.Clear();
                        WriteSuccessMessage("Login successful! Welcome " + enteredUserName + "!");
                        
                        double bal = GetUserAccountBalance(enteredUserName, enteredPassword);
                        WriteBlankLineAndMessage("Your balance is " + bal.ToString("C"));
                        
                        // Show product list
                        while (true)
                        {
                            WriteBlankLineAndMessage("What would you like to buy?");
                            WriteProductList();

                            // Prompt for user input
                            WriteBlankLineAndMessage("Enter a number:");
                            string answer = Console.ReadLine();
                            int num = Convert.ToInt32(answer);
                            num = num - 1; // Subtract 1 from number, because array is zero based and user list is one based.

                            // The last item is the 'exit' choice.
                            if (num == numberOfProducts)
                            {
                                // Update balance
                                foreach (var usr in usrs)
                                {
                                    // Check that name and password match
                                    if (usr.Name == enteredUserName && usr.Pwd == enteredPassword)
                                    {
                                        usr.Bal = bal;
                                    }
                                }

                                // Write out new balance
                                string json = JsonConvert.SerializeObject(usrs, Formatting.Indented);
                                File.WriteAllText(@"Data/Users.json", json);

                                // Write out new quantities
                                string json2 = JsonConvert.SerializeObject(prods, Formatting.Indented);
                                File.WriteAllText(@"Data/Products.json", json2);


                                // Prevent console from closing
                                WriteBlankLineAndMessage("Press Enter key to exit");
                                Console.ReadLine();
                                return;
                            }
                            else
                            {
                                WriteBlankLineAndMessage("You want to buy: " + prods[num].Name);
                                WriteMessage("Your balance is " + bal.ToString("C"));
                                WriteMessage("Enter amount to purchase:");
                                answer = Console.ReadLine();
                                int userEnteredQuantityToBuy = Convert.ToInt32(answer);

                                // Check if balance - quantity * price is less than 0
                                if (bal - prods[num].Price * userEnteredQuantityToBuy < 0)
                                {
                                    Console.Clear();
                                    WriteErrorMessage("You do not have enough money to buy that.");
                                    continue;
                                }

                                // This is the one defect I fixed.  I left other defects/improvements as is.
                                if (prods[num].Qty < userEnteredQuantityToBuy)
                                {
                                    Console.Clear();
                                    WriteErrorMessage("Sorry, " + prods[num].Name + " is out of stock");
                                    continue;
                                }

                                // Check if quantity is greater than zero
                                if (userEnteredQuantityToBuy > 0)
                                {
                                    // Balance = Balance - Price * Quantity
                                    bal = bal - prods[num].Price * userEnteredQuantityToBuy;

                                    // Quanity = Quantity - Quantity
                                    prods[num].Qty = prods[num].Qty - userEnteredQuantityToBuy;

                                    Console.Clear();
                                    WriteSuccessMessage("You bought " + userEnteredQuantityToBuy + " " + prods[num].Name);
                                    WriteMessage("Your new balance is " + bal.ToString("C"));
                                }
                                else
                                {
                                    // Quantity is less than zero
                                    Console.Clear();
                                    WriteBlankLineAndMessage("Purchase cancelled");
                                }
                            }
                        }
                    }
                    else
                    {
                        // Invalid Password
                        Console.Clear();
                        WriteErrorMessage("You entered an invalid password.");
                        
                        goto Login;
                    }
                }
                else
                {
                    // Invalid User
                    Console.Clear();
                    WriteBlankLineAndMessage("You entered an invalid user.");
                    
                    goto Login;
                }
            }

            // Prevent console from closing
            WriteBlankLineAndMessage("Press Enter key to exit");
            Console.ReadLine();
        }

        private static void WriteProductList()
        {
            for (int i = 0; i < numberOfProducts; i++)
            {
                WriteMessage(i + 1 + ": " + products[i].Name + " (" + products[i].Price.ToString("C") + ")");
            }
            WriteMessage(numberOfProducts + 1 + ": Exit");
        }

        private static bool IsValidUser(string userName)
        {
            bool isValidUser = false;

            for (int i = 0; i < numberOfUsers; i++)
            {
                User user = users[i];
                if (user.Name == userName)
                {
                    isValidUser = true;
                }
            }
            return isValidUser;
        }

        private static bool IsValidPassword(string userName, string password)
        {
            bool isValidPassword = false;

            for (int i = 0; i < numberOfUsers; i++)
            {
                User user = users[i];
                if (user.Name == userName && user.Pwd == password)
                {
                    isValidPassword = true;
                }
            }
            return isValidPassword;
        }

        private static double GetUserAccountBalance(string userName, string password)
        {
            double userAccountBalance = 0.0;

            for (int i = 0; i < numberOfUsers; i++)
            {
                User user = users[i];
                if (user.Name == userName && user.Pwd == password)
                {
                    userAccountBalance = user.Bal;
                }
            }
            return userAccountBalance;
        }

        private static void WriteBlankLineAndMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            Console.WriteLine(message);   
        }

        private static void WriteMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message);
        }

        private static void WriteErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine(message);
        }

        private static void WriteSuccessMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine(message);
        }
    }
}
