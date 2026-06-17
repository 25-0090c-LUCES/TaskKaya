using System;
using System.Threading;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Threading.Tasks;

namespace TaskKaya
{
    internal class Program
    {
        static void CreateFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                var fl = File.Create(fileName);
                fl.Close();
            }
        }
        static void CreateFile2(string fileName)
        {
            if (!File.Exists(fileName))
            {
                var fl = File.Create(fileName);
                fl.Close();
            }
        }

        static void Main(string[] args)
        {
            CreateFile("LogInCredentials.txt");
            bool running = true;
            while (running)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Clear();
                Console.WriteLine(@"
                               ████████╗ █████╗ ███████╗██╗  ██╗██╗  ██╗ █████╗ ██╗   ██╗ █████╗ 
                               ╚══██╔══╝██╔══██╗██╔════╝██║ ██╔╝██║ ██╔╝██╔══██╗╚██╗ ██╔╝██╔══██╗
                                  ██║   ███████║███████╗█████╔╝ █████╔╝ ███████║ ╚████╔╝ ███████║
                                  ██║   ██╔══██║╚════██║██╔═██╗ ██╔═██╗ ██╔══██║  ╚██╔╝  ██╔══██║
                                  ██║   ██║  ██║███████║██║  ██╗██║  ██╗██║  ██║   ██║   ██║  ██║
                                  ╚═╝   ╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝   ╚═╝  ╚═╝

      ");
                Console.ResetColor();
                Console.WriteLine("[1]. Login");
                Console.WriteLine("[2]. Register");
                Console.WriteLine("[3]. Exit");
                Console.Write("Enter  number of choice: ");
                string input = Console.ReadLine();
                if (!int.TryParse(input, out int choice))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n[ERROR] Please enter number of choice.");
                    Thread.Sleep(1000);
                    Console.ResetColor();
                    continue;
                }
                switch (choice)
                {
                    case 1:
                        try
                        {
                            string[] LogInFiles = File.ReadAllLines("LogInCredentials.txt");

                            var (username, password) = SplitFile(LogInFiles);
                            LoginProcess(username, password);
                        }
                        catch (FileNotFoundException)
                        {
                            Console.WriteLine("Login file not found.");
                            Console.ReadKey();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                            Console.ReadKey();
                        }
                        break;
                    case 2:
                        Register();
                        break;
                    case 3:
                        
                        Console.WriteLine("\nThank you for using TaskKaya. GoodBye!");
                        Console.ReadKey();
                        running = false;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n[ERROR] Invalid choice try again.");
                        Thread.Sleep(1000);
                        Console.ResetColor();
                        break;
                }
            }
        }
        static (string[] username, string[] password) SplitFile(string[] LogInFiles)
        {
            // Split ng LoginFiles
            string[] username = new string[LogInFiles.Length];
            string[] password = new string[LogInFiles.Length];

            for (int i = 0; i < LogInFiles.Length; i++)
            {
                string[] parts = LogInFiles[i].Split(',');
                if (parts.Length >= 2)
                {
                    username[i] = parts[0];
                    password[i] = parts[1];
                }
            }
            return (username, password);
        }
        static void LoginProcess(string[] username, string[] password)
        {
            try
            {
                bool loggedin = false;
                int attempts = 0;
                string loggedInUser = "";

                while (!loggedin && attempts < 2)
                {
                    Console.Clear();
                    Console.WriteLine("---------------------------------------------------------------");
                    Console.WriteLine("LOGIN");
                    Console.WriteLine("Type B at any time to return to the Main Menu.");
                    Console.WriteLine("---------------------------------------------------------------");

                    Console.Write("Enter Username: ");
                    string usern = Console.ReadLine();

                    if (usern.ToUpper() == "B")
                    {
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(usern))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n[ERROR] Username cannot be empty. Try again.");
                        Thread.Sleep(1100);
                        Console.ResetColor();
                        continue;
                    }

                    Console.Write("Enter Password: ");
                    string userpass = Console.ReadLine();

                    if (userpass.ToUpper() == "B")
                    {
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(userpass))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n[ERROR] Password cannot be empty. Try Again.");
                        Thread.Sleep(1100);
                        Console.ResetColor();
                        continue;
                    }

                    for (int i = 0; i < username.Length; i++)
                    {
                        if (usern == username[i] && userpass == password[i])
                        {
                            
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("\nLogin successful! Directing to Dashboard ");
                            Thread.Sleep(1100);
                            Console.ResetColor();

                            loggedin = true;
                            loggedInUser = username[i];
                            break;
                        }
                    }

                    if (!loggedin)
                    {
                        attempts++;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n[ERROR] Invalid credentials. Try again...");
                        Thread.Sleep(1000);
                        Console.ResetColor();

                    }
                }

                if (!loggedin)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("---------------------------------------------------------------");
                    Console.WriteLine("Maximum login attempts reached!Please try again later.");
                    Console.WriteLine("Returning to main menu.");
                    Console.WriteLine("---------------------------------------------------------------");
                    Thread.Sleep(1300);
                    Console.ResetColor();

                }
                else
                {
                    ShowMenu(loggedInUser);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred during login.");
                Console.WriteLine("Error: " + ex.Message);
                Console.ReadKey();
            }
        }
        static void Register()
        {
            string username = "";
            string password = "";
            string confirmPassword = "";

            // USERNAME
            while (true)
            {
                Console.Clear();

                Console.WriteLine("---------------------------------------------------------------");
                Console.WriteLine("                        REGISTER");
                Console.WriteLine("             Type B at any time to go back");
                Console.WriteLine("---------------------------------------------------------------");

                Console.Write("Enter Username: ");
                username = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(username) &&
                    username.ToUpper() == "B")
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(username))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n[ERROR] Username cannot be empty. Try again.");
                    Thread.Sleep(1000);
                    Console.ResetColor();
                    continue;
                }

                bool exists = false;

                string[] LogInFiles = File.ReadAllLines("LogInCredentials.txt");

                for (int i = 0; i < LogInFiles.Length; i++)
                {
                    if (LogInFiles[i] != "")
                    {
                        string[] parts = LogInFiles[i].Split(',');

                        if (parts.Length >= 2)
                        {
                            if (parts[0].ToUpper() == username.ToUpper())
                            {
                                exists = true;
                                break;
                            }
                        }
                    }
                }

                if (exists)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n[ERROR] Username already exists. Please choose another username.");
                    Thread.Sleep(1000);
                    Console.ResetColor();
                    continue;
                }

                break;
            }

            // PASSWORD
            while (true)
            {
                Console.Clear();

                Console.WriteLine("---------------------------------------------------------------");
                Console.WriteLine("                        REGISTER");
                Console.WriteLine("             Type B at any time to go back");
                Console.WriteLine("---------------------------------------------------------------");

                Console.WriteLine($"Enter Username: {username}");
                Console.Write("Enter Password: ");

                password = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(password) &&
                    password.ToUpper() == "B")
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n[ERROR] Password cannot be empty. Try again.");
                    Thread.Sleep(1000);
                    Console.ResetColor();
                    continue;
                }

                break;
            }

            // CONFIRM PASSWORD
            while (true)
            {
                Console.Clear();

                Console.WriteLine("---------------------------------------------------------------");
                Console.WriteLine("                        REGISTER");
                Console.WriteLine("             Type B at any time to go back");
                Console.WriteLine("---------------------------------------------------------------");

                Console.WriteLine($"Enter Username: {username}");
                Console.Write("Confirm Password: ");

                confirmPassword = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(confirmPassword) &&
                    confirmPassword.ToUpper() == "B")
                {
                    return;
                }

                if (confirmPassword != password)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n[ERROR] Passwords do not match. Try again.");
                    Thread.Sleep(1000);
                    Console.ResetColor();
                    continue;
                }

                break;
            }

            string newEntry = username + "," + password;

            File.AppendAllText(
                "LogInCredentials.txt",
                newEntry + Environment.NewLine);


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nRegistration successful! Returning to the Main Menu.");
            Thread.Sleep(1200);
            Console.ResetColor();
            return;
        }
        static (string[] task, string[] employer, string[] location, string[] rate) SplitTask(string[] TaskList)
        {
            // Split ng LoginFiles
            string[] task = new string[TaskList.Length];
            string[] employer = new string[TaskList.Length];
            string[] location = new string[TaskList.Length];
            string[] rate = new string[TaskList.Length];


            for (int i = 0; i < TaskList.Length; i++)
            {
                string[] parts = TaskList[i].Split(',');
                if (parts.Length >= 4)
                {
                    task[i] = parts[0];
                    employer[i] = parts[1];
                    location[i] = parts[2];
                    rate[i] = parts[3];

                }
            }
            return (task, employer, location, rate);
        }
        static void ShowMenu(string loggedInUser)
        {
            bool running = true;
            CreateFile2("TaskList.txt");
            while (running)
            {
                Console.Clear();

                string[] TaskList = File.ReadAllLines("TaskList.txt");
                var (task, employer, location, rate) = SplitTask(TaskList);
                Console.WriteLine("[1]. Find a Job");
                Console.WriteLine("[2]. Add a Job");
                Console.WriteLine("[3]. View Notifications");
                Console.WriteLine("[4]. View Job Applicants");
                Console.WriteLine("[5]. Log Out");
                Console.Write("Enter number of choice: ");
                string input = Console.ReadLine();

                if (!int.TryParse(input, out int choice))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n[ERROR] Please enter a valid number.");
                    Thread.Sleep(1000);
                    Console.ResetColor();
                    continue;
                }
                switch (choice)
                {
                    case 1:
                        FindJob(task, employer, location, rate);
                        break;
                    case 2:
                        AddJob();
                        break;
                    case 3:
                        ViewNotifcations(loggedInUser);
                        break;
                    case 4:
                        ViewJobApplicants(loggedInUser);
                        break;
                    case 5:
                        Console.Clear();
                        running = false;
                        Console.WriteLine("Logged out successfully. Thank you for using TaskKaya. GoodBye!");
                        Thread.Sleep(1000);
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n[ERROR] Invalid choice try again.");
                        Thread.Sleep(1000);
                        Console.ResetColor();
                        break;
                }
            }
        }
        static void FindJob(string[] task, string[] employer, string[] location, string[] rate)
        {

        }
        static void AddJob()
        {

        }
        static void ViewNotifcations(string loggedInUser)
        {

        }
        static void ViewJobApplicants(string loggedInUser)
        {

        }
    }
}