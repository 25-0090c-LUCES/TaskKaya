
namespace TaskKaya
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Net.NetworkInformation;
    using System.Reflection;
    using System.Threading;

    internal class Program
    {
        // USERNAMES AND PASSWORDS ONLY
        static List<string> UserNames = new List<string>();
        static List<string> UserPasswords = new List<string>();
        static List<string> UserLoc = new List<string>();
        static List<string> UserConNum = new List<string>();

        // PARALLEL LISTS FOR JOBS (Replaces Public Class Job)
        static List<string> JobIDs = new List<string>();
        static List<string> JobTitles = new List<string>();
        static List<string> JobBudgets = new List<string>();
        static List<string> JobEmployers = new List<string>();
        static List<string> JobWorkers = new List<string>();
        static List<string> JobStatuses = new List<string>();
        static List<string> JobRatings = new List<string>();
        static List<string> JobLocations = new List<string>();

        // Engine queues and stacks for simple text parsing
        static Queue<string> LiveNotifications = new Queue<string>();
        static Stack<string> TransactionHistory = new Stack<string>();

        static void Main(string[] args)
        {
            Console.Title = "Universal Job Dashboard";
            LoadData();

            while (true)
            {
                string currentUser = LoginPortal();
                if (currentUser == "") continue;

                RunUniversalDashboard(currentUser);
            }
        }

        // ==========================================
        // MAIN MENU LOOP (Loop-based Alert Counting)
        // ==========================================
        static void RunUniversalDashboard(string username)
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("===============================================================================");
                Console.WriteLine("  DASHBOARD | USER: " + username.ToUpper());
                Console.WriteLine("===============================================================================");
                Console.ResetColor();

                // Manual loop instead of LINQ count
                int alertCount = 0;
                foreach (string notif in LiveNotifications)
                {
                    string[] parts = notif.Split('|');
                    if (parts[0].Equals(username, StringComparison.OrdinalIgnoreCase))
                    {
                        alertCount++;
                    }
                }

                if (alertCount > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(" 🔔 [ALERT] You have (" + alertCount + ") unread message(s) in your Inbox!\n");
                    Console.ResetColor();
                }

                Console.WriteLine(" [1] Find a Job / Apply");
                Console.WriteLine(" [2] Post a Job");
                Console.WriteLine(" [3] Review Applicants & Completions");
                Console.WriteLine(" [4] Track My Ongoing Work");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" [5] Check Notifications Inbox (Approved/Finished/Declined)");
                Console.ResetColor();
                Console.WriteLine(" [6] View History Ledger");
                Console.WriteLine(" [0] Logout");
                Console.WriteLine("-------------------------------------------------------------------------------");

                int choice = ReadInt("Enter number of choice: ");

                switch (choice)
                {
                    case 1: BrowseJobsBoard(username); break;
                    case 2: PostJob(username); break;
                    case 3: ReviewAndVerifyTasks(username); break;
                    case 4: TrackWorkerContracts(username); break;
                    case 5: CheckAndDisplayNotifications(username); break;
                    case 6: ViewGlobalLedger(); break;
                    case 0: return;
                }
            }
        }

        // ==========================================
        // CORE FEATURES (Using Loops & Synchronized Indexes)
        // ==========================================
        static void PostJob(string username)
        {
            Console.Clear();
            Console.WriteLine("=== POST A JOB ===");

            Console.Write("Job Title: ");
            string title = Console.ReadLine().Trim();

            Console.Write("Job Location: ");
            string location = Console.ReadLine().Trim();

            Console.Write("Budget (PHP): ");
            string budget = Console.ReadLine().Trim();

            if (string.IsNullOrWhiteSpace(title) ||
                string.IsNullOrWhiteSpace(location) ||
                string.IsNullOrWhiteSpace(budget))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!] Fields cannot be empty.");
                Console.ResetColor();
                Pause();
                return;
            }

            string newID = "JOB" + new Random().Next(100, 999);

            JobIDs.Add(newID);
            JobTitles.Add(title);
            JobLocations.Add(location);
            JobBudgets.Add(budget);
            JobEmployers.Add(username);
            JobWorkers.Add("None");
            JobStatuses.Add("AVAILABLE");
            JobRatings.Add("N/A");

            SaveData();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n[SUCCESS] Job " + newID + " posted successfully!");
            Console.ResetColor();
            Pause();
        }

        static void BrowseJobsBoard(string username)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== FIND A JOB ===");
                Console.WriteLine("[1] View All Jobs");
                Console.WriteLine("[2] Filter by Location");
                Console.WriteLine("[0] Return");

                int choice = ReadInt("Enter number of choice: ");

                if (choice == 0)
                    return;

                string filterLocation = "";

                if (choice == 2)
                {
                    Console.Write("Enter Location: ");
                    filterLocation = Console.ReadLine().Trim();
                }

                Console.Clear();
                Console.WriteLine("=== AVAILABLE JOBS ===");

                int count = 0;

                for (int i = 0; i < JobIDs.Count; i++)
                {
                    bool available =
                        JobStatuses[i] == "AVAILABLE" &&
                        !JobEmployers[i].Equals(username, StringComparison.OrdinalIgnoreCase);

                    bool locationMatch =
                        choice == 1 ||
                        JobLocations[i].Equals(filterLocation, StringComparison.OrdinalIgnoreCase);

                    if (available && locationMatch)
                    {
                        Console.WriteLine(
                            $"[ID: {JobIDs[i]}] {JobTitles[i]} | " +
                            $"Location: {JobLocations[i]} | " +
                            $"Budget: PHP {JobBudgets[i]} | " +
                            $"Employer: {JobEmployers[i]}"
                        );

                        count++;
                    }
                }

                if (count == 0)
                {
                    Console.WriteLine("\nNo jobs found.");
                    Pause();
                    continue;
                }

                Console.Write("\nEnter Job ID to apply for (or press Enter to cancel): ");
                string targetId = Console.ReadLine().Trim().ToUpper();

                if (targetId == "")
                    continue;

                int matchIndex = -1;

                for (int i = 0; i < JobIDs.Count; i++)
                {
                    if (JobIDs[i] == targetId &&
                        JobStatuses[i] == "AVAILABLE")
                    {
                        matchIndex = i;
                        break;
                    }
                }

                if (matchIndex != -1)
                {

                    AddNotification(
     JobEmployers[matchIndex],
     "APPLIED",
     username + ";" + JobIDs[matchIndex]
 );

                    SaveData();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n[SUCCESS] Application submitted! Waiting for Employer review.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n[ERROR] Job ID not found or unavailable.");
                    Console.ResetColor();
                }

                Pause();
            }
        }

        // Helper helper to dynamically check total completed reviews to form a user rating baseline
        static string GetUserAverageRating(string workerName)
        {
            int totalStars = 0;
            int jobCount = 0;

            for (int i = 0; i < JobIDs.Count; i++)
            {
                if (JobWorkers[i].Equals(workerName, StringComparison.OrdinalIgnoreCase) && JobStatuses[i] == "COMPLETED")
                {
                    string stars = JobRatings[i];
                    if (!string.IsNullOrEmpty(stars) && stars != "N/A")
                    {
                        totalStars += stars.Length; // Length gives count of '★' characters
                        jobCount++;
                    }
                }
            }

            if (jobCount == 0) return "No performance reviews yet";
            int average = (int)Math.Round((double)totalStars / jobCount);
            return $"{new string('★', average)} ({jobCount} jobs completed)";
        }
       
        static void ReviewAndVerifyTasks(string username)
        {
            Console.Clear();
            Console.WriteLine("=== REVIEW HUB ===");

            // =================================================================
            // 1. DISPLAY PENDING APPLICANTS (Dynamically read from Queue)
            // =================================================================
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n--- PENDING JOB APPLICANTS ---");
            Console.ResetColor();
            int applicantCount = 0;
            // Keeps track of the mapped applicant info: "workerName|jobID"
            List<string> applicantMapping = new List<string>();

            foreach (string notif in LiveNotifications)
            {
                string[] parts = notif.Split('|');
                // Format check: TargetUser | Category | Payload (Worker|JobID)
                if (parts.Length >= 3 && parts[1] == "APPLIED" && parts[0].Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    string[] payload = parts[2].Split(';');
                    if (payload.Length >= 2)
                    {
                        string workerName = payload[0];
                        string targetJobId = payload[1];

                        // Find the index of the job to ensure it's still AVAILABLE
                        int jobIdx = -1;
                        for (int j = 0; j < JobIDs.Count; j++)
                        {
                            if (JobIDs[j] == targetJobId)
                            {
                                jobIdx = j;
                                break;
                            }
                        }

                        if (jobIdx != -1 && JobStatuses[jobIdx] == "AVAILABLE")
                        {
                            string workerScore = GetUserAverageRating(workerName);
                            Console.WriteLine($" -> [{applicantCount + 1}] Job ID: {targetJobId} ({JobTitles[jobIdx]}) | Applicant: {workerName} | Rating: {workerScore}");

                            applicantMapping.Add(parts[2]); // Stores "workerName|jobID"
                            applicantCount++;
                        }
                    }
                }
            }
            if (applicantCount == 0) Console.WriteLine("No active entry applications at this time.");

            // =================================================================
            // 2. DISPLAY COMPLETIONS PENDING PAYMENT VERIFICATION
            // =================================================================
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n--- COMPLETIONS PENDING PAYMENT VERIFICATION ---");
            Console.ResetColor();
            int pendingCount = 0;
            for (int i = 0; i < JobIDs.Count; i++)
            {
                if (JobEmployers[i].Equals(username, StringComparison.OrdinalIgnoreCase) && JobStatuses[i] == "PENDING_VERIFICATION")
                {
                    Console.WriteLine(" -> [ID: " + JobIDs[i] + "] " + JobTitles[i] + " | Worker: " + JobWorkers[i] + " | Cost: PHP " + JobBudgets[i]);
                    pendingCount++;
                }
            }
            if (pendingCount == 0) Console.WriteLine("No completed tasks requiring code handshakes.");

            // Exit early if nothing needs attention
            if (applicantCount == 0 && pendingCount == 0)
            {
                Pause();
                return;
            }

            // =================================================================
            // 3. DECISION ROUTER (Applicants vs Completions)
            // =================================================================
            Console.Write("\nDo you want to evaluate an Applicant [A] or a Completion Verification [C]? (A/C): ");
            string choiceMode = Console.ReadLine().Trim().ToUpper();

            // PHASE A: PROCESSING A SPECIFIC APPLICANT
            if (choiceMode == "A" && applicantCount > 0)
            {
                int appIndex = ReadInt("Enter number of applicant choice index: ") - 1;
                if (appIndex >= 0 && appIndex < applicantMapping.Count)
                {
                    string[] payload = applicantMapping[appIndex].Split(';');
                    string workerName = payload[0];
                    string targetJobId = payload[1];

                    int matchIndex = -1;
                    for (int j = 0; j < JobIDs.Count; j++)
                    {
                        if (JobIDs[j] == targetJobId) { matchIndex = j; break; }
                    }

                    if (matchIndex != -1)
                    {
                        Console.Clear();
                        Console.WriteLine($"Evaluating Applicant for Job: {JobIDs[matchIndex]} ({JobTitles[matchIndex]})");
                        Console.WriteLine($"Applicant Username: {workerName}");
                        Console.WriteLine($"Applicant Rating Baseline: {GetUserAverageRating(workerName)}");
                        Console.WriteLine("-------------------------------------------------------------------------------");
                        Console.WriteLine(" [1] Accept Applicant & Move Contract into Active Production State");
                        Console.WriteLine(" [2] Decline/Reject Applicant application");
                        int decision = ReadInt("Enter decision action index: ");

                        if (decision == 1)
                        {
                            // Assign worker and switch status to ONGOING (removes it from Browse Board)
                            JobStatuses[matchIndex] = "ONGOING";
                            JobWorkers[matchIndex] = workerName;

                            AddNotification(workerName, "APPROVED", $"Great news! Employer '{username}' has ACCEPTED you for the job '{JobTitles[matchIndex]}' (ID: {JobIDs[matchIndex]}). You can start work now.");

                            // Remove application card cleanly out of queue
                            RemoveApplicationNotification(username, workerName, targetJobId);

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("\n[SUCCESS] Contract finalized. Worker can now safely track ongoing duties.");
                            Console.ResetColor();
                        }
                        else if (decision == 2)
                        {
                            // The job naturally remains AVAILABLE; nothing clears out for other applicants!
                            AddNotification(workerName, "DECLINED", $"Your application for '{JobTitles[matchIndex]}' (ID: {JobIDs[matchIndex]}) was declined by the employer.");

                            // Delete this applicant's bid from queue
                            RemoveApplicationNotification(username, workerName, targetJobId);

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("\n[INFO] Application rejected. Other applicants can still be reviewed.");
                            Console.ResetColor();
                        }
                        SaveData();
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection index.");
                }
                Pause();
            }
            // PHASE B: EVALUATING COMPLETED SUBMISSIONS
            else if (choiceMode == "C" && pendingCount > 0)
            {
                Console.Write("Enter Job ID to evaluate completion: ");
                string targetId = Console.ReadLine().Trim().ToUpper();

                int matchIndex = -1;
                for (int i = 0; i < JobIDs.Count; i++)
                {
                    if (JobIDs[i] == targetId &&
                        JobEmployers[i].Equals(username, StringComparison.OrdinalIgnoreCase) &&
                        JobStatuses[i] == "PENDING_VERIFICATION")
                    {
                        matchIndex = i;
                        break;
                    }
                }

                if (matchIndex == -1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n[ERROR] Match system index mismatch or permissions violation.");
                    Console.ResetColor();
                    Pause();
                    return;
                }

                Console.Clear();
                Console.WriteLine("Evaluating Job Output: " + JobIDs[matchIndex] + " (" + JobTitles[matchIndex] + ")");
                Console.WriteLine("-------------------------------------------------------------------------------");
                Console.WriteLine(" [1] Approve Completion & Release Payment");
                Console.WriteLine(" [2] Reject & Request Revision");
                int choice = ReadInt("Enter number of choice: ");

                if (choice == 1)
                {
                    JobStatuses[matchIndex] = "COMPLETED";

                    Console.Write("\nRate worker performance (1 to 5 Stars): ");
                    int stars = ReadInt("");
                    if (stars < 1) stars = 1; if (stars > 5) stars = 5;
                    JobRatings[matchIndex] = new string('★', stars);

                    string stackPayload = JobIDs[matchIndex] + "|" + JobTitles[matchIndex] + "|" + JobWorkers[matchIndex] + "|" + JobBudgets[matchIndex] + "|" + JobRatings[matchIndex];
                    TransactionHistory.Push(stackPayload);

                    AddNotification(JobWorkers[matchIndex], "FINISHED", "Payment Released! '" + username + "' marked your work on '" + JobTitles[matchIndex] + "' as complete. Rating: " + JobRatings[matchIndex]);
                    SaveData();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n[SUCCESS] Job finalized and paid.");
                    Console.ResetColor();
                }
                else if (choice == 2)
                {
                    JobStatuses[matchIndex] = "ONGOING";
                    AddNotification(JobWorkers[matchIndex], "DECLINED", "[!] Work Rejected: '" + username + "' requested edits for job '" + JobTitles[matchIndex] + "'.");
                    SaveData();
                    Console.WriteLine("\n[INFO] Sent back to worker for corrections.");
                }
                Pause();
            }
            else
            {
                Console.WriteLine("Invalid selection mode or no items available in that section.");
                Pause();
            }
        }
        static void RemoveApplicationNotification(string employer, string worker, string jobId)
        {
            List<string> retainedNotifs = new List<string>();
            string targetPayload = worker + ";" + jobId;
            foreach (string n in LiveNotifications)
            {
                string[] parts = n.Split('|');
                // Match Employer, category APPLIED, and the matching worker|job payload
                if (parts[0].Equals(employer, StringComparison.OrdinalIgnoreCase) &&
                    parts[1] == "APPLIED" &&
                    parts[2] == targetPayload)
                {
                    continue; // Skip it to drop/delete it
                }
                retainedNotifs.Add(n);
            }

            LiveNotifications.Clear();
            foreach (string n in retainedNotifs) LiveNotifications.Enqueue(n);
        }

        static void TrackWorkerContracts(string username)
        {
            Console.Clear();
            Console.WriteLine("=== TRACK MY ONGOING WORK ===");

            int ongoingCount = 0;
            for (int i = 0; i < JobIDs.Count; i++)
            {
                if (JobWorkers[i].Equals(username, StringComparison.OrdinalIgnoreCase) && JobStatuses[i] == "ONGOING")
                {
                    Console.WriteLine(" -> [ID: " + JobIDs[i] + "] " + JobTitles[i] + " | Earnings: PHP " + JobBudgets[i] + " | Client: " + JobEmployers[i]);
                    ongoingCount++;
                }
            }

            if (ongoingCount == 0)
            {
                Console.WriteLine("\nYou have no active approved jobs currently in active progress state.");
                Pause();
                return;
            }

            Console.Write("\nEnter Job ID to submit for approval: ");
            string targetId = Console.ReadLine().Trim().ToUpper();

            int matchIndex = -1;
            for (int i = 0; i < JobIDs.Count; i++)
            {
                if (JobIDs[i] == targetId && JobWorkers[i].Equals(username, StringComparison.OrdinalIgnoreCase) && JobStatuses[i] == "ONGOING")
                {
                    matchIndex = i;
                    break;
                }
            }

            if (matchIndex != -1)
            {
                JobStatuses[matchIndex] = "PENDING_VERIFICATION";
                AddNotification(JobEmployers[matchIndex], "FINISHED", "Review Required: '" + username + "' completed '" + JobTitles[matchIndex] + "' (ID: " + JobIDs[matchIndex] + ").");
                SaveData();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n[SUCCESS] Job submitted! Waiting for employer approval.");
                Console.ResetColor();
                Pause();
            }
        }

        // ==========================================
        // PERSISTENT NOTIFICATION INBOX ENGINE
        // ==========================================
        static void CheckAndDisplayNotifications(string username)
        {
            while (true)
            {
                List<string> personalNotifications = new List<string>();
                foreach (string notif in LiveNotifications)
                {
                    if (notif.StartsWith(username + "|"))
                    {
                        personalNotifications.Add(notif);
                    }
                }

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("===============================================================================");
                Console.WriteLine("               NOTIFICATIONS INBOX | TOTAL ALERTS: " + personalNotifications.Count);
                Console.WriteLine("===============================================================================");
                Console.ResetColor();
                Console.WriteLine(" [1] View Folder: Approved/Applied Jobs");
                Console.WriteLine(" [2] View Folder: Jobs Finished / Submission Reviews");
                Console.WriteLine(" [3] View Folder: Declined / Returned Job Tasks");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" [4] Clear ALL My Notifications");
                Console.ResetColor();
                Console.WriteLine(" [0] Return to Main Menu");
                Console.WriteLine("-------------------------------------------------------------------------------");

                int choice = ReadInt("Enter number of choice: ");
                if (choice == 0) return;

                if (choice == 4)
                {
                    List<string> otherUsersNotifs = new List<string>();
                    foreach (string n in LiveNotifications)
                    {
                        if (!n.StartsWith(username + "|"))
                        {
                            otherUsersNotifs.Add(n);
                        }
                    }

                    LiveNotifications.Clear();
                    foreach (string n in otherUsersNotifs) LiveNotifications.Enqueue(n);

                    SaveData();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n[SUCCESS] Inbox completely cleared.");
                    Console.ResetColor();
                    Pause();
                    continue;
                }

                string targetedCategoryCode = "";
                if (choice == 1) targetedCategoryCode = "APPROVED";
                else if (choice == 2) targetedCategoryCode = "FINISHED";
                else if (choice == 3) targetedCategoryCode = "DECLINED";
                else continue;

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("=== FOLDER INBOX: " + targetedCategoryCode + " ===");
                Console.WriteLine("-------------------------------------------------------------------------------");

                int displayCount = 0;
                foreach (string notif in personalNotifications)
                {
                    string[] parts = notif.Split('|');
                    if (parts[1] == targetedCategoryCode)
                    {
                        Console.WriteLine(" * " + parts[2]);
                        displayCount++;
                    }
                }

                if (displayCount == 0)
                {
                    Console.WriteLine("No notifications in this folder.");
                }

                Console.WriteLine("-------------------------------------------------------------------------------");
                Console.ResetColor();
                Pause();
            }
        }

        static void AddNotification(string target, string category, string message)
        {
            LiveNotifications.Enqueue(target + "|" + category + "|" + message);
            SaveData();
        }

        static void ViewGlobalLedger()
        {
            Console.Clear();
            Console.WriteLine("=== HISTORY LEDGER ===");
            if (TransactionHistory.Count == 0)
            {
                Console.WriteLine("\nNo completed entries logged yet.");
            }
            else
            {
                foreach (string log in TransactionHistory)
                {
                    string[] p = log.Split('|');
                    Console.WriteLine(" -> ID: " + p[0] + " | Job: " + p[1] + " | Worker: " + p[2] + " | Paid: PHP " + p[3] + " | Rating: " + p[4]);
                }
            }
            Pause();
        }

        // ==========================================
        // ACCESS CONTROL & STORAGE UTILITIES
        // ==========================================
        static string LoginPortal()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkGreen;

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
                Console.Write("Enter number of choice: ");

                string input = Console.ReadLine();

                if (!int.TryParse(input, out int choice))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n[ERROR] Please enter a valid number.");
                    Console.ResetColor();
                    Thread.Sleep(1000);
                    continue;
                }

                if (choice == 1)
                {
                    Console.Clear();
                    Console.Write("Enter Username: ");
                    string u = Console.ReadLine().Trim();

                    Console.Write("Enter Password: ");
                    string p = Console.ReadLine().Trim();

                    for (int i = 0; i < UserNames.Count; i++)
                    {
                        if (UserNames[i].Equals(u, StringComparison.OrdinalIgnoreCase)
                            && UserPasswords[i] == p)
                        {
                            return UserNames[i];
                        }
                    }

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n[ERROR] Invalid username or password.");
                    Console.ResetColor();
                    Pause();
                }
                else if (choice == 2)
                {
                    string u = "";
                    string p = "";
                    string l = "";
                    string c = "";

                    // USERNAME
                    while (true)
                    {
                        Console.Clear();
                        Console.WriteLine("=========== REGISTER NEW ACCOUNT ===========");

                        Console.Write("Enter Username: ");
                        u = Console.ReadLine().Trim();

                        bool exists = false;

                        for (int i = 0; i < UserNames.Count; i++)
                        {
                            if (UserNames[i].Equals(u, StringComparison.OrdinalIgnoreCase))
                            {
                                exists = true;
                                break;
                            }
                        }

                        if (exists)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n[ERROR] Username already exists.");
                            Console.ResetColor();
                            Thread.Sleep(1000);
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(u))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n[ERROR] Username cannot be empty.");
                            Console.ResetColor();
                            Thread.Sleep(1000);
                            continue;
                        }

                        break;
                    }

                    // PASSWORD
                    while (true)
                    {
                        Console.Clear();
                        Console.WriteLine("=========== REGISTER NEW ACCOUNT ===========");

                        Console.WriteLine($"Enter Username: {u}");
                        Console.Write("Enter Password: ");

                        p = Console.ReadLine().Trim();

                        if (string.IsNullOrWhiteSpace(p))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n[ERROR] Password cannot be empty.");
                            Console.ResetColor();
                            Thread.Sleep(1000);
                            continue;
                        }

                        break;
                    }

                    // LOCATION
                    while (true)
                    {
                        Console.Clear();
                        Console.WriteLine("=========== REGISTER NEW ACCOUNT ===========");

                        Console.WriteLine($"Enter Username: {u}");
                        Console.WriteLine($"Enter Password: {p}");

                        Console.Write("Enter Location: ");

                        l = Console.ReadLine().Trim();

                        if (string.IsNullOrWhiteSpace(l))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n[ERROR] Location cannot be empty.");
                            Console.ResetColor();
                            Thread.Sleep(1000);
                            continue;
                        }

                        break;
                    }

                    // CONTACT NUMBER
                    while (true)
                    {
                        Console.Clear();
                        Console.WriteLine("=========== REGISTER NEW ACCOUNT ===========");

                        Console.WriteLine($"Enter Username: {u}");
                        Console.WriteLine($"Enter Password: {p}");
                        Console.WriteLine($"Enter Location: {l}");

                        Console.Write("Enter Contact Number: ");

                        c = Console.ReadLine().Trim();

                        if (c.Length != 11)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n[ERROR] Contact number must be 11 digits.");
                            Console.ResetColor();
                            Thread.Sleep(1000);
                            continue;
                        }

                        break;
                    }

                    UserNames.Add(u);
                    UserPasswords.Add(p);
                    UserLoc.Add(l);
                    UserConNum.Add(c);

                    SaveData();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n[SUCCESS] Account created successfully!");
                    Console.WriteLine("You may now log in.");
                    Console.ResetColor();

                    Thread.Sleep(1500);
                }
                else if (choice == 3)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nThank you for using TaskKaya!");
                    Console.ResetColor();

                    Thread.Sleep(1000);
                    Environment.Exit(0);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n[ERROR] Invalid choice.");
                    Console.ResetColor();

                    Thread.Sleep(1000);
                }
            }
        }

        static void SaveData()
        {
            List<string> userLines = new List<string>();
            for (int i = 0; i < UserNames.Count; i++)
                userLines.Add(UserNames[i] + "," + UserPasswords[i] + "," + UserLoc[i] + "," + UserConNum[i]);
            File.WriteAllLines("users_universal.txt", userLines);

            List<string> jobLines = new List<string>();
            for (int i = 0; i < JobIDs.Count; i++)
                jobLines.Add(JobIDs[i] + "|" + JobTitles[i] + "|" + JobLocations[i] +
             "|" + JobBudgets[i] + "|" + JobEmployers[i] +
             "|" + JobWorkers[i] + "|" + JobStatuses[i] +
             "|" + JobRatings[i]);
            File.WriteAllLines("jobs_universal.txt", jobLines);

            File.WriteAllLines("notifications_universal.txt", LiveNotifications.ToArray());
            File.WriteAllLines("history_universal.txt", TransactionHistory.ToArray());
        }

        static void LoadData()
        {
            if (File.Exists("users_universal.txt"))
            {
                UserNames.Clear();
                UserPasswords.Clear();
                UserLoc.Clear();
                UserConNum.Clear();
                foreach (string line in File.ReadAllLines("users_universal.txt"))
                {
                    string[] p = line.Split(',');
                    if (p.Length == 4)
                    {
                        UserNames.Add(p[0]);
                        UserPasswords.Add(p[1]);
                        UserLoc.Add(p[2]);
                        UserConNum.Add(p[3]);
                    }
                }
            }
            if (File.Exists("jobs_universal.txt"))
            {
                JobIDs.Clear();
                JobTitles.Clear();
                JobBudgets.Clear();
                JobEmployers.Clear();
                JobWorkers.Clear();
                JobStatuses.Clear();
                JobRatings.Clear();
                JobLocations.Clear();
                foreach (string line in File.ReadAllLines("jobs_universal.txt"))
                {
                    string[] p = line.Split('|');
                    if (p.Length == 8)
                    {
                        JobIDs.Add(p[0]);
                        JobTitles.Add(p[1]);
                        JobLocations.Add(p[2]);
                        JobBudgets.Add(p[3]);
                        JobEmployers.Add(p[4]);
                        JobWorkers.Add(p[5]);
                        JobStatuses.Add(p[6]);
                        JobRatings.Add(p[7]);
                    }
                }
            }
            if (File.Exists("notifications_universal.txt"))
            {
                LiveNotifications.Clear();
                foreach (string line in File.ReadAllLines("notifications_universal.txt"))
                    if (!string.IsNullOrWhiteSpace(line)) LiveNotifications.Enqueue(line);
            }
            if (File.Exists("history_universal.txt"))
            {
                TransactionHistory.Clear();
                string[] lines = File.ReadAllLines("history_universal.txt");
                for (int i = lines.Length - 1; i >= 0; i--)
                {
                    if (!string.IsNullOrWhiteSpace(lines[i]))
                        TransactionHistory.Push(lines[i]);
                }
            }
        }

        static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int result)) return result;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!] Numbers only.");
                Console.ResetColor();
            }
        }

        static void Pause()
        {
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }
    }
}
