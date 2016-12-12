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
        private static IUserAwardLogic userAwardLogic;

        private static ConsoleColor original;

        static void Main(string[] args)
        {
            try
            {
                userAwardLogic = Provider.Instance.UserAwardLogic;

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
                        case "4":
                            ShowAwards();
                            break;
                        case "5":
                            AddAward();
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
            catch (ConfigurationErrorsException ex)
            {
                Console.WriteLine("Error in configuration file:");
                ShowColorText(ex.BareMessage, ConsoleColor.Red);
                return;
            }
        }


        private static void ShowMenu()
        {
            Console.WriteLine("Enter action:");
            Console.WriteLine("1 - Show all users");
            Console.WriteLine("2 - Add new user");
            Console.WriteLine("3 - Delete user");
            Console.WriteLine("4 - Show all awards");
            Console.WriteLine("5 - Add award");
            Console.WriteLine("0 - Exit");
        }


        private static void AddAward()
        {
            Console.WriteLine("Enter award's title:");
            string title = Console.ReadLine();
            try
            {
                if (userAwardLogic.AddAward(title))
                    ShowColorText("Award was added", ConsoleColor.Green);
                else
                    ShowColorText("Unknown error! Can't add award", ConsoleColor.Red);
            }
            catch (Exception)
            {
                ShowColorText("Unknown error! Can't add award", ConsoleColor.Red);
            }
        }


        private static void ShowAwards()
        {
            try
            {
                IEnumerable<AwardDTO> awards = userAwardLogic.GetAllAwards();
                int number = 0;
                foreach (var award in awards)
                {
                    Console.WriteLine($"{++number}. {award.Title}");
                }
            }
            catch
            {
                ShowColorText("Error! Can't show awards", ConsoleColor.Red);
            }
        }


        private static void ShowUsers()
        {
            try
            {
                IEnumerable<UserDTO> users = userAwardLogic.GetAllUsers().OrderBy(user => user.Name);
                List<UserVM> usersWithAwards = new List<UserVM>();
                foreach (var user in users)
                {
                    usersWithAwards.Add(new UserVM
                    {
                        Id = user.Id,
                        Name = user.Name,
                        BirthDate = user.BirthDate,
                        Awards = userAwardLogic.GetUserAwards(user.Id).ToList(),
                    });
                }


                int num = 0;
                foreach (var user in usersWithAwards)
                {
                    Console.WriteLine($"User {++num}:");
                    Console.WriteLine($"Name: {user.Name}");
                    Console.WriteLine($"Birth date: {user.BirthDate.ToShortDateString()}");
                    Console.WriteLine($"Age: {user.Age}");
                    Console.WriteLine("Awards:");
                    foreach (var award in user.Awards)
                    {
                        Console.WriteLine($"-{award.Title}");
                    }
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
                if (userAwardLogic.AddUser(name, birthDate))
                    ShowColorText($"User was added", ConsoleColor.Green);
                else
                    ShowColorText("Unknown error! Can't add user", ConsoleColor.Red);
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
                ShowColorText("Unknown error! Can't add user", ConsoleColor.Red);
            }
        }


        private static void DeleteUser()
        {
            var users = userAwardLogic.GetAllUsers().OrderBy(user=>user.Name);
            int max = users.Count();

            ShowUsers();
            int number;
            if (!IsValidNumber(max, out number))
                return;
            number--;
            Guid id = users.Skip(number).Select(user => user.Id).First();

            try
            {
                if (userAwardLogic.DeleteUser(id))
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
