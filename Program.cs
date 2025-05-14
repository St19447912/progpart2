using System;
using System.IO;
using System.Media;
using System.Collections.Generic;

namespace progpart1
{
    class CyberSecurityChatbot
    {
        // Dictionary to store keyword-responses
        private Dictionary<string, string> responses;

        // Dictionary to store tips
        private Dictionary<string, List<string>> tips;

        // List to store user interests
        private List<string> topicsOfInterest;

        // Store the user's name and last discussed topic
        private string userName;
        private string lastTopic;

        // Delegate for chatbot responses
        private delegate void ResponseHandler(string message);
        private ResponseHandler PrintResponse;

        public CyberSecurityChatbot()
        {
            // Keyword dictionary
            responses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "password", "A password is a secret key used to protect accounts. Use a mix of letters, numbers, and symbols." },
                { "cybersecurity", "Cybersecurity involves protecting systems and networks from digital attacks." },
                { "how are you", "I'm doing great! How can I help you today?" },
                { "what is your purpose", "My purpose is to help you with cybersecurity-related matters!" },
                { "phishing", "Phishing is a scam where attackers trick you into providing personal information." },
                { "malware", "Malware is malicious software designed to harm your system." },
                { "hardware", "Hardware refers to the physical components of a computer." },
                { "protect information", "To stay safe online, use strong passwords and enable two-factor authentication." },
                { "privacy", "Privacy is your right to control access to your personal information online." },
                { "scam", "Scams are fraudulent attempts to deceive people for financial or personal gain." }
            };

            // Tips for each topic
            tips = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "password", new List<string>{
                    "Use a mix of uppercase, lowercase, numbers, and symbols.",
                    "Avoid using personal information.",
                    "Change your passwords regularly." } },
                { "phishing", new List<string>{
                    "Don't click on suspicious links.",
                    "Check sender email addresses.",
                    "Avoid sharing personal info online." } },
                { "malware", new List<string>{
                    "Keep your antivirus software updated.",
                    "Avoid downloading files from untrusted sources.",
                    "Regularly scan your computer." } },
                { "privacy", new List<string>{
                    "Adjust your social media privacy settings.",
                    "Avoid oversharing online.",
                    "Use encrypted messaging apps." } },
                { "scam", new List<string>{
                    "Be skeptical of too-good-to-be-true offers.",
                    "Never give out personal information to strangers.",
                    "Verify sources before taking any action." } }
            };

            topicsOfInterest = new List<string>();

            // Lambda expression assigned to delegate
            PrintResponse = (message) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nChatbot: {message}");
                Console.ResetColor();
            };
        }

        public void Run()
        {
            LoadLastSession();
            PlaySound("greeting.wav");
            DisplayAsciiLogo();
            WelcomeUser();
            StartChat();
            SaveSession();
        }

        private void PlaySound(string filePath)
        {
            try
            {
                SoundPlayer player = new SoundPlayer(filePath);
                player.PlaySync();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error playing audio: " + ex.Message);
                Console.ResetColor();
            }
        }

        private void DisplayAsciiLogo()
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ascii-art.txt");
                string asciiArt = File.ReadAllText(filePath);
                Console.WriteLine(asciiArt);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error loading ASCII logo: " + ex.Message);
                Console.ResetColor();
            }
        }

        private void WelcomeUser()
        {
            Console.WriteLine("\n===============================");
            Console.WriteLine("          Chatbot          ");
            Console.WriteLine("===============================");
        }

        private void StartChat()
        {
            userName = GetUserInput("\nChatbot: Hey! Welcome to the cybersecurity chatbot, what is your name?");
            Console.WriteLine($"\nChatbot: Hey {userName}! What do you need help with?");

            string userInput;
            do
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"\n{userName}: ");
                Console.ResetColor();
                userInput = Console.ReadLine()?.ToLower();

                if (userInput == "exit")
                {
                    if (ConfirmExit()) return;
                    continue;
                }

                if (CheckSentiment(userInput)) continue;
                if (CheckTipsRequest(userInput)) continue;
                if (CheckInterestStatement(userInput)) continue;

                ProcessUserInput(userInput);
            } while (true);
        }

        private string GetUserInput(string prompt)
        {
            string input;
            do
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(prompt);
                Console.ResetColor();
                Console.Write("User: ");
                input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Name cannot be empty! Please enter your name.");
                    Console.ResetColor();
                }
            } while (string.IsNullOrWhiteSpace(input));

            return input;
        }

        private bool CheckSentiment(string input)
        {
            if (input.Contains("i'm worried") || input.Contains("i am worried"))
            {
                PrintResponse("I'm sorry you're feeling worried. I'm here to help you.");
                return true;
            }
            if (input.Contains("i'm confused") || input.Contains("i am confused"))
            {
                PrintResponse("Don't worry, let's break things down step by step.");
                return true;
            }
            if (input.Contains("i'm frustrated") || input.Contains("i am frustrated"))
            {
                PrintResponse("I understand. Let's try to solve this together.");
                return true;
            }
            return false;
        }

        private bool CheckTipsRequest(string input)
        {
            foreach (var key in tips.Keys)
            {
                if (input.Contains(key + " tips"))
                {
                    PrintResponse($"Here are some tips for {key}:");
                    foreach (var tip in tips[key])
                    {
                        Console.WriteLine("- " + tip);
                    }
                    return true;
                }
            }
            return false;
        }

        private bool CheckInterestStatement(string input)
        {
            foreach (var key in responses.Keys)
            {
                if ((input.Contains("i'm interested in") || input.Contains("i am interested in")) && input.Contains(key))
                {
                    if (!topicsOfInterest.Contains(key)) topicsOfInterest.Add(key);
                    PrintResponse($"Great! I'll remember that you're interested in {key}.");
                    return true;
                }
            }
            return false;
        }

        private void ProcessUserInput(string input)
        {
            foreach (var key in responses.Keys)
            {
                if (input.Contains(key))
                {
                    lastTopic = key;
                    PrintRandomResponse(responses[key]);
                    return;
                }
            }
            PrintResponse("Sorry, I can't help you with that. I'm here to assist with cybersecurity topics.");
        }

        private void PrintRandomResponse(string baseResponse)
        {
            string[] variations =
            {
                baseResponse,
                baseResponse + " Let me know if you need more info.",
                "Sure! " + baseResponse,
                baseResponse + " Hope that helps!"
            };
            Random rand = new Random();
            string randomResponse = variations[rand.Next(variations.Length)];
            PrintResponse(randomResponse);
        }

        private bool ConfirmExit()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\nChatbot: Are you sure you want to exit? (yes/no): ");
            Console.ResetColor();

            string response = Console.ReadLine()?.ToLower();
            if (response == "yes")
            {
                Console.WriteLine($"Chatbot: Goodbye {userName}! Last topic we discussed was '{lastTopic}'. Stay safe online.");
                return true;
            }
            Console.WriteLine("Chatbot: Alright, let's continue!");
            return false;
        }

        private void SaveSession()
        {
            try
            {
                File.WriteAllLines("memory.txt", new[] { userName, lastTopic });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving session: " + ex.Message);
            }
        }

        private void LoadLastSession()
        {
            try
            {
                if (File.Exists("memory.txt"))
                {
                    var lines = File.ReadAllLines("memory.txt");
                    if (lines.Length >= 2)
                    {
                        userName = lines[0];
                        lastTopic = lines[1];
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"[Memory Recall] Welcome back {userName}! Last time we spoke about '{lastTopic}'.");
                        Console.ResetColor();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading previous session: " + ex.Message);
            }
        }
    }

    class Program
    {
        static void Main()
        {
            CyberSecurityChatbot chatbot = new CyberSecurityChatbot();
            chatbot.Run();
        }
    }
}
