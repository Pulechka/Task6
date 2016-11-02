using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersKeeper.Entities;
using UsersKeeper.Logic;

namespace UsersKeeper.UI
{
    class Program
    {
        private static UserLogic userLogic;
        private static ConsoleColor original;

        static void Main(string[] args)
        {
            original = Console.ForegroundColor;
            try
            {
                userLogic = new UserLogic();
            }
            catch (ConfigurationErrorsException ex)
            {
                Console.WriteLine("Error in configuration file:");
                ShowColorText(ex.BareMessage, ConsoleColor.Red);
                return;
            }

            while (true)
            {
                ShowMenu();
                string userInput = Console.ReadLine();
                Console.WriteLine();
                switch (userInput)
                {
                    case "1":
                        ShowUsers();
                        break;
                    case "2":
                        AddUser();
                        break;
                    case "3":
                        DeleteUser();
                        break;
                    case "0":
                        return;
                    default:
                        ShowColorText("Incorrect command!", ConsoleColor.Red);
                        break;
                }
                Console.WriteLine();
            }
        }

        private static void ShowMenu()
        {
            Console.WriteLine("1 - Show all users");
            Console.WriteLine("2 - Add new user");
            Console.WriteLine("3 - Delete user");
            Console.WriteLine("0 - Exit");
        }

        private static void DeleteUser()
        {
            ShowUsers();
            int id = 0;
            while (id == 0)
            {
                Console.Write("Enter user ID for delete: ");
                int.TryParse(Console.ReadLine(), out id);
                if (id == 0)
                {
                    ShowColorText("Incorrect ID format!", ConsoleColor.Red);
                }
            }

            try
            {
                if (userLogic.Delete(id))
                {
                    ShowColorText($"User with ID {id} was deleted", ConsoleColor.Green);
                }
                else
                {
                    ShowColorText($"Error! Can't delete user with ID {id}", ConsoleColor.Red);
                }

            }
            catch
            {
                ShowColorText("Error! Can't delete user", ConsoleColor.Red);
            }
        }

        private static void AddUser()
        {
            Console.WriteLine("Enter data for new user:");

            Console.Write("Name: ");
            string name = Console.ReadLine();

            DateTime birthDate = DateTime.MinValue;
            while (birthDate == DateTime.MinValue)
            {
                Console.Write("Birth date: ");
                DateTime.TryParse(Console.ReadLine(), out birthDate);
                if (birthDate == DateTime.MinValue)
                    ShowColorText("Incorrect date format!", ConsoleColor.Red);
            }

            try
            {
                int id = userLogic.Add(name, birthDate);
                ShowColorText($"User with id {id} was added", ConsoleColor.Green);
            }
            catch (ArgumentException ex)
            {
                ShowColorText("Error! Can't add user with such data", ConsoleColor.Red);
                switch (ex.ParamName)
                {
                    case "birthDate":
                        ShowColorText("Invalid birth date", ConsoleColor.DarkRed);
                        break;
                    case "name":
                        ShowColorText("Invalid name", ConsoleColor.DarkRed);
                        break;
                }
            }
            catch (Exception)
            {
                ShowColorText("Error! Can't add user", ConsoleColor.Red);
            }

        }

        private static void ShowUsers()
        {
            try
            {
                IEnumerable<User> users = userLogic.GetAll().OrderBy(user => user.Id);
                foreach (var user in users)
                {
                    Console.WriteLine($"User {user.Id}:");
                    Console.WriteLine($"Name: {user.Name}");
                    Console.WriteLine($"Birth date: {user.BirthDate.ToShortDateString()}");
                    Console.WriteLine($"Age: {user.Age}");
                    Console.WriteLine();
                }
            }
            catch
            {
                ShowColorText("Error! Can't show users", ConsoleColor.Red);
            }
        }

        private static void ShowColorText(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = original;
        }
    }
}
