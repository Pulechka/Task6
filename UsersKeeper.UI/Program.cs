using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersKeeper.BllContracts;
using UsersKeeper.Entities;
using UsersKeeper.Providers;

namespace UsersKeeper.ConsoleUI
{
    class Program
    {
        private static IUserLogic userLogic;
        private static ConsoleColor original;

        static void Main(string[] args)
        {
            try
            {
                userLogic = Provider.UserLogic;
            }
            catch (ConfigurationErrorsException ex)
            {
                Console.WriteLine("Error in configuration file:");
                ShowColorText(ex.BareMessage, ConsoleColor.Red);
                return;
            }

            original = Console.ForegroundColor;

            while (true)
            {
                ShowMenu();
                string userInput = Console.ReadLine();
                Console.Clear();

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
            Console.WriteLine("Enter action:");
            Console.WriteLine("1 - Show all users");
            Console.WriteLine("2 - Add new user");
            Console.WriteLine("3 - Delete user");
            Console.WriteLine("0 - Exit");
        }


        private static void ShowUsers()
        {
            try
            {
                IEnumerable<User> users = userLogic.GetAll().OrderBy(user => user.Name);
                int num = 0;
                foreach (var user in users)
                {
                    Console.WriteLine($"User {++num}:");
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


        private static void AddUser()
        {
            Console.WriteLine("Enter data for new user:");

            Console.Write("Name: ");
            string name = Console.ReadLine();

            DateTime birthDate;
            IsValidBirthDate(out birthDate);

            try
            {
                Guid id = userLogic.Add(name, birthDate);
                ShowColorText($"User was added", ConsoleColor.Green);
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


        private static void DeleteUser()
        {
            var users = userLogic.GetAll().OrderBy(user=>user.Name);
            int max = users.Count();

            ShowUsers();
            int number;
            if (!IsValidNumber(max, out number))
                return;
            number--;
            Guid id = users.Skip(number).Select(user => user.Id).First();

            try
            {
                if (userLogic.Delete(id))
                    ShowColorText($"User with number {++number} was deleted", ConsoleColor.Green);
                else
                    ShowColorText($"Error! Can't delete user with number {++number}", ConsoleColor.Red);
            }
            catch
            {
                ShowColorText("Error! Can't delete user", ConsoleColor.Red);
            }
        }


        private static bool IsValidNumber(int max, out int number)
        {
            number = 0;
            Console.Write("Enter user number for delete: ");
            int.TryParse(Console.ReadLine(), out number);
            if (number == 0)
            {
                ShowColorText("Incorrect number format!", ConsoleColor.Red);
                return false;
            }
            else if (number < 0 || number > max)
            {
                ShowColorText("Invalid user number!", ConsoleColor.Red);
                return false;
            }
            return true;
        }


        private static bool IsValidBirthDate(out DateTime birthDate)
        {
            birthDate = DateTime.MinValue;
            Console.Write("Birth date: ");
            DateTime.TryParse(Console.ReadLine(), out birthDate);
            if (birthDate == DateTime.MinValue)
            {
                ShowColorText("Incorrect date format!", ConsoleColor.Red);
                return false;
            }
            return true;
        }


        private static void ShowColorText(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = original;
        }
    }
}
