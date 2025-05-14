using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Threading;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Collections.Generic;

namespace CyberSecurityAwarenessChatBot
{
    public class run_interfaceApp
    {
        // Delegate declarations for handling responses and matching questions
        public delegate string ResponseHandler(string sentiment);
        public delegate bool QuestionMatcher(string question);

        // Class variables
        string title = @"Welcome to Cyber Security Awareness Bot";
        private readonly Random _random = new Random(); // Random generator for varied responses
        private string userName; // Stores user's name
        private List<string> userInterests = new List<string>(); // Tracks user's interests
        private MemoryRecall memory = new MemoryRecall(); // Memory system instance

        // Conversation history tracking
        private List<string> conversationHistory = new List<string>();

        // Tracks how many times each topic has been responded to
        private Dictionary<string, int> responseCounters = new Dictionary<string, int>();

        // Constructor to starts the application
        public run_interfaceApp()
        {
            Logo_design(); // Display ASCII logo
            welcome_message(); // Play welcome sound
            Menu(); // Start main interaction loop
        }

        // Converts logo image to ASCII art
        private void Logo_design()
        {
            // Get and adjust file path
            string paths = AppDomain.CurrentDomain.BaseDirectory;
            string new_path = paths.Replace("bin\\Debug\\", "");
            string full_path = Path.Combine(new_path, "pic.png");

            Console.WriteLine(full_path);
            Bitmap Logo = new Bitmap(full_path);
            Logo = new Bitmap(Logo, new Size(120, 50)); // Resize logo

            // Convert each pixel to ASCII character based on brightness
            for (int height = 0; height < Logo.Height; height++)
            {
                for (int width = 0; width < Logo.Width; width++)
                {
                    Color pixelColor = Logo.GetPixel(width, height);
                    int gray = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    char asciiChar = gray > 250 ? '.' : gray > 150 ? '*' : gray > 100 ? 'o' : gray > 50 ? '#' : '@';
                    Console.Write(asciiChar);
                }
                Console.WriteLine();
            }
        }

        // Plays welcome sound
        private void welcome_message()
        {
            string full_location = AppDomain.CurrentDomain.BaseDirectory;
            string new_path = full_location.Replace("bin\\Debug\\", "");

            try
            {
                string full_path = Path.Combine(new_path, "greeting.wav");
                using (SoundPlayer play = new SoundPlayer(full_path))
                {
                    play.PlaySync(); // Play sound synchronously
                }
            }
            catch (Exception error)
            {
                Console.Write(error.Message); // Handle sound playback errors
            }
        }

        // Validates user name contains only letters and spaces
        private bool IsValidName(string name)
        {
            foreach (char c in name)
            {
                if (!char.IsLetter(c) && c != ' ')
                {
                    return false;
                }
            }
            return true;
        }

        // Main menu and interaction loop
        private void Menu()
        {
            // Display title and header
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(title);
            Console.WriteLine("\n+==================================================================+");
            Console.ResetColor();

            // Get user name
            Console.Write("\nWhat's your name? ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n+==================================================================+");
            Console.ResetColor();

            userName = Console.ReadLine()?.Trim();

            // Validate name input
            while (string.IsNullOrEmpty(userName) || !IsValidName(userName))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please enter a valid name (letters only, no numbers or special characters).");
                Console.ResetColor();
                userName = Console.ReadLine()?.Trim();
            }

            // Store user name in memory
            memory.StoreUserInfo(userName, "name");
            AddToHistory($"User entered name: {userName}");

            // Greet user
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n+=====================================================================+");
            Console.ResetColor();

            Console.WriteLine($"\nHello, {userName}! I'm here to help you stay safe online.");
            AddToHistory($"Bot greeted user: {userName}");
            Thread.Sleep(1000);

            // Display menu options
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n+=====================================================================+");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("+=======================================================================+");
            Console.ResetColor();

            Console.WriteLine("\nYou can ask me about:");
            Console.WriteLine(" 1 Password Safety");
            Console.WriteLine(" 2 Phishing Attacks");
            Console.WriteLine(" 3 Safe Browsing Practices");
            Console.WriteLine(" 4 General Cybersecurity");
            Console.WriteLine("Type your question or type 'exit' to leave.");
            Console.WriteLine("Type 'history' to view our conversation history.\n");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("+========================================================================+");
            Console.ResetColor();

            // Main conversation loop
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.Write("You: ");
                Console.ResetColor();
                string userInput = Console.ReadLine()?.Trim().ToLower();

                // Handle empty input
                if (string.IsNullOrEmpty(userInput))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("+=================================================================+");
                    Console.WriteLine("Please enter a valid question.");
                    Console.WriteLine("+==================================================================+");
                    Console.ResetColor();
                    continue;
                }

                // Exit command
                if (userInput == "exit")
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("+==================================================================+");
                    Console.WriteLine("Goodbye! Stay safe online.");
                    Console.WriteLine("+==================================================================");
                    Console.ResetColor();
                    AddToHistory("User exited the chat");
                    break;
                }

                // Show history command
                if (userInput == "history")
                {
                    ShowHistory();
                    continue;
                }

                // Process user input
                AddToHistory($"User asked: {userInput}");
                string sentiment = DetectSentiment(userInput); // Detect user's mood

                // Simulate typing indicator
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("ChatBot is typing");
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(500);
                    Console.Write(".");
                }
                Console.WriteLine();
                Console.ResetColor();

                // Random response delay for realism
                int delay = _random.Next(1000, 3000);
                Thread.Sleep(delay);

                // Generate and display response
                string response = GenerateResponse(userInput, sentiment);
                AddToHistory($"Bot responded: {response}");
                TypeWriterEffect(response); // Animated typing effect
            }
        }

        // Adds entry to conversation history with timestamp
        private void AddToHistory(string entry)
        {
            conversationHistory.Add($"{DateTime.Now:HH:mm:ss}: {entry}");
        }

        // Displays conversation history
        private void ShowHistory()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n+=== Conversation History ===+");
            if (conversationHistory.Count == 0)
            {
                Console.WriteLine("No history yet. Start chatting to build history!");
            }
            else
            {
                foreach (var entry in conversationHistory)
                {
                    Console.WriteLine(entry);
                }
            }
            Console.WriteLine("+===========================+");
            Console.ResetColor();
        }

        // Detects user sentiment from input text
        private string DetectSentiment(string input)
        {
            input = input.ToLower();
            // Check for worried sentiment
            if (input.Contains("worried") || input.Contains("scared") || input.Contains("afraid") ||
                input.Contains("nervous") || input.Contains("anxious"))
            {
                return "worried";
            }
            // Check for frustrated sentiment
            else if (input.Contains("angry") || input.Contains("mad") || input.Contains("frustrated") ||
                     input.Contains("annoyed") || input.Contains("upset"))
            {
                return "frustrated";
            }
            // Check for happy sentiment
            else if (input.Contains("happy") || input.Contains("excited") || input.Contains("glad") ||
                     input.Contains("pleased") || input.Contains("thrilled"))
            {
                return "happy";
            }
            // Check for sad sentiment
            else if (input.Contains("sad") || input.Contains("depressed") || input.Contains("unhappy") ||
                     input.Contains("miserable") || input.Contains("down"))
            {
                return "sad";
            }
            // Check for curious sentiment
            else if (input.Contains("interested") || input.Contains("curious") || input.Contains("want to know") ||
                     input.Contains("wondering") || input.Contains("tell me about"))
            {
                return "curious";
            }

            return "neutral"; // Default sentiment
        }

        // Generates appropriate response based on user input
        private string GenerateResponse(string question, string sentiment)
        {
            // Delegate handler for password-related questions
            ResponseHandler passwordHandler = (sent) =>
            {
                // Track user interest in passwords
                if (!userInterests.Contains("password"))
                {
                    userInterests.Add("password");
                    memory.StoreUserInfo("password", "interest");
                }

                // Track response count for password topic
                if (!responseCounters.ContainsKey("password"))
                {
                    responseCounters["password"] = 0;
                }
                responseCounters["password"]++;

                // Multiple response sets for variety
                var responses = new List<List<string>>()
                {
                    new List<string> {
                        "Make sure to use strong, unique passwords for each account.",
                        "A good password should be at least 12 characters long...",
                        "Consider using a password manager to keep track...",
                        "Never share your passwords with anyone..."
                    },
                    new List<string> {
                        "Password security is crucial. Did you know...",
                        "A passphrase can be more secure than a password...",
                        "Two-factor authentication adds an extra layer...",
                        "Avoid using personal information in passwords..."
                    },
                    new List<string> {
                        "For better security, consider using multi-word passphrases...",
                        "Biometric authentication can be more secure...",
                        "Password managers can generate and store complex passwords...",
                        "Security questions should be treated like passwords..."
                    }
                };

                // Select response set based on counter
                int responseSet = responseCounters["password"] % responses.Count;
                return AdjustForSentiment(responses[responseSet][_random.Next(responses[responseSet].Count)], sent);
            };

            // Delegate handler for phishing-related questions
            ResponseHandler phishingHandler = (sent) =>
            {
                // Track user interest in phishing
                if (!userInterests.Contains("phishing"))
                {
                    userInterests.Add("phishing");
                    memory.StoreUserInfo("phishing", "interest");
                }

                // Track response count for phishing topic
                if (!responseCounters.ContainsKey("phishing"))
                {
                    responseCounters["phishing"] = 0;
                }
                responseCounters["phishing"]++;

                // Multiple response sets for variety
                var responses = new List<List<string>>()
                {
                    new List<string> {
                        "Be cautious of emails asking for personal information...",
                        "Phishing emails often create a sense of urgency...",
                        "Check the sender's email address carefully...",
                        "If an email seems suspicious, don't click any links..."
                    },
                    new List<string> {
                        "Spear phishing targets specific individuals...",
                        "Some phishing attempts come via text messages...",
                        "Look for poor grammar and spelling...",
                        "Hover over links to see the actual URL..."
                    },
                    new List<string> {
                        "Advanced phishing attacks can even spoof legitimate websites...",
                        "Some phishing emails use your real name...",
                        "Be wary of emails that claim your account will be closed...",
                        "Even if an email looks like it's from someone you know..."
                    }
                };

                // Select response set based on counter
                int responseSet = responseCounters["phishing"] % responses.Count;
                return AdjustForSentiment(responses[responseSet][_random.Next(responses[responseSet].Count)], sent);
            };

            // Dictionary mapping question patterns to response handlers
            var responseHandlers = new Dictionary<QuestionMatcher, ResponseHandler>
            {
                { q => q.Contains("password"), passwordHandler },
                { q => q.Contains("phishing") || q.Contains("scam"), phishingHandler },
                { q => q.Contains("privacy") || q.Contains("data protection"), (sent) =>
                    {
                        // Track user interest in privacy
                        if (!userInterests.Contains("privacy"))
                        {
                            userInterests.Add("privacy");
                            memory.StoreUserInfo("privacy", "interest");
                        }

                        // Track response count for privacy topic
                        if (!responseCounters.ContainsKey("privacy"))
                        {
                            responseCounters["privacy"] = 0;
                        }
                        responseCounters["privacy"]++;

                        // Multiple response sets for variety
                        var responses = new List<List<string>>()
                        {
                            new List<string> {
                                "Review privacy settings on your social media accounts...",
                                "Be careful about what personal information you share online...",
                                "Use privacy-focused browsers and search engines...",
                                "Consider using a VPN to protect your online privacy..."
                            },
                            new List<string> {
                                "Browser cookies can track your activity across sites...",
                                "Many apps request more permissions than they need...",
                                "Metadata in photos and documents can reveal...",
                                "Encrypt sensitive files and communications..."
                            },
                            new List<string> {
                                "Some websites use fingerprinting techniques...",
                                "Your online purchases and searches can create...",
                                "Even anonymized data can sometimes be re-identified...",
                                "Privacy laws vary by region..."
                            }
                        };

                        // Select response set based on counter
                        int responseSet = responseCounters["privacy"] % responses.Count;
                        return AdjustForSentiment(responses[responseSet][_random.Next(responses[responseSet].Count)], sent);
                    }
                },
                // General purpose handlers
                { q => q.Contains("your name") || q.Contains("who are you"),
                  _ => "I'm your Cybersecurity Awareness Chatbot, here to help you stay safe online!" },
                { q => q.Contains("remember") || q.Contains("what do you know"),
                  _ => memory.RecallUserInfo(userName) ?? "I remember we've talked before..." },
                { q => q.Contains("interest") || q.Contains("like") || q.Contains("prefer"),
                  _ => userInterests.Count > 0
                      ? $"Based on our conversation, you seem interested in: {string.Join(", ", userInterests)}..."
                      : "You haven't mentioned any specific interests yet..." },
                { q => q.Contains("history"),
                  _ => "You can type 'history' at any time to see our conversation history." }
            };

            // Try to find matching handler for the question
            foreach (var handler in responseHandlers)
            {
                if (handler.Key(question))
                {
                    return handler.Value(sentiment);
                }
            }

            // Fallback to generic responses if no specific handler matches
            return user_replies(question);
        }

        // Adjusts response based on detected sentiment
        private string AdjustForSentiment(string response, string sentiment)
        {
            switch (sentiment)
            {
                case "worried":
                    return "I understand this might be concerning. " + response + " Remember, being aware is the first step to staying safe.";
                case "frustrated":
                    return "I hear your frustration. Cybersecurity can be complex, but " + response.ToLower();
                case "happy":
                    return "Great to see your enthusiasm! " + response;
                case "sad":
                    return "I'm sorry you're feeling this way. " + response + " Taking small steps can help improve your security.";
                case "curious":
                    return "That's a great question! " + response;
                default:
                    return response;
            }
        }

        // Fallback method for unrecognized questions
        private string user_replies(string question)
        {
            ArrayList replies = new ArrayList(); // Stores possible responses
            ArrayList ignore = new ArrayList(); // Words to ignore when matching

            store_ignore(ignore); // Load words to ignore
            store_replies(replies, ignore); // Load possible responses

            // Split question into words and filter out ignored words
            string[] store_word = question.ToLower().Split(' ');
            ArrayList store_final_words = new ArrayList();

            for (int count = 0; count < store_word.Length; count++)
            {
                if (!ignore.Contains(store_word[count]))
                {
                    store_final_words.Add(store_word[count]);
                }
            }

            bool found = false;
            string message = string.Empty;

            // Try to find matching responses based on remaining keywords
            for (int counting = 0; counting < store_final_words.Count; counting++)
            {
                for (int count = 0; count < replies.Count; count++)
                {
                    if (replies[count].ToString().ToLower().Contains(store_final_words[counting].ToString().ToLower()))
                    {
                        message += replies[count] + "\n";
                        found = true;
                    }
                }
            }

            // Return matched responses or default message
            return found ? message.Trim() : "I'm sorry, I don't understand that question. Please ask about cybersecurity topics.";
        }

        // Creates typewriter effect for chatbot responses
        private void TypeWriterEffect(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("ChatBot: ");

            int baseDelay = _random.Next(20, 50); // Base typing speed

            // Print each character with slight random delay
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(baseDelay + _random.Next(-10, 20));

                // Longer pause for punctuation
                if (new[] { '.', ',', '!', '?' }.Contains(c))
                {
                    Thread.Sleep(_random.Next(100, 300));
                }
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        // Loads possible responses into the replies ArrayList
        private void store_replies(ArrayList replies, ArrayList ignore)
        {
            // Password security responses
            replies.Add("Password security requires strong, unique passwords and regular changes.");
            replies.Add("Multi-factor authentication adds an extra layer of security beyond passwords.");

            // Phishing responses
            replies.Add("Phishing attacks often use fake emails to steal sensitive information.");
            replies.Add("Never click on suspicious links or download attachments from unknown emails.");

            // General security responses
            replies.Add("Ransomware encrypts files and demands payment for their release.");
            replies.Add("Social engineering manipulates people into revealing confidential information.");
            replies.Add("Malware includes viruses, worms, and trojans that harm computer systems.");

            // Browsing safety responses
            replies.Add("Avoid entering personal information on untrusted or unknown websites.");
            replies.Add("Always check if a website uses HTTPS before entering sensitive data.");

            // Chatbot capability responses
            replies.Add("I can explain cybersecurity concepts and best practices.");
            replies.Add("Ask me about common cyber threats and how to avoid them.");

            // Greeting responses
            replies.Add("Hello! How can I help with cybersecurity today?");
            replies.Add("Hi there! You can ask me about phishing, online security, or password safety.");

            // More specific responses
            replies.Add("Phishing emails often have urgent requests or too-good-to-be-true offers.");
            replies.Add("Hover over links to check their real destination before clicking.");
            replies.Add("Keep software updated to protect against known vulnerabilities.");

            ignore.Add(" "); // Ensure spaces are ignored
        }

        // Loads words to ignore when matching questions to responses
        private void store_ignore(ArrayList ignore)
        {
            ignore.Add("tell");
            ignore.Add("me");
            ignore.Add("about");
            ignore.Add("are");
            ignore.Add("you");
            ignore.Add("your");
            ignore.Add("whats");
            ignore.Add("can");
            ignore.Add("i");
            ignore.Add("ask");
            ignore.Add("");
            ignore.Add("the");
            ignore.Add("a");
            ignore.Add("an");
            ignore.Add("how");
            ignore.Add("what");
            ignore.Add("where");
            ignore.Add("when");
            ignore.Add("why");
            ignore.Add("Attacks");
            ignore.Add("Safety");
        }
    }
}

    
    
