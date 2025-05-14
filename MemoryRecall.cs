using System;
using System.Collections.Generic;
using System.IO;

namespace CyberSecurityAwarenessChatBot
{
    public class MemoryRecall
    {
        private string memoryFilePath; // Path to memory storage file

        public MemoryRecall()
        {
            // Set up memory file path
            string fullPath = AppDomain.CurrentDomain.BaseDirectory;
            string newPath = fullPath.Replace("bin\\Debug\\", "");
            memoryFilePath = Path.Combine(newPath, "ChatbotMemory.txt");

            // Create file if it doesn't exist
            if (!File.Exists(memoryFilePath))
            {
                File.Create(memoryFilePath).Close();
            }
        }

        // Stores user information in memory file
        public void StoreUserInfo(string info, string infoType)
        {
            try
            {
                List<string> existingLines = new List<string>();
                if (File.Exists(memoryFilePath))
                {
                    existingLines = new List<string>(File.ReadAllLines(memoryFilePath));
                }

                bool found = false;
                // Update existing info if type exists
                for (int i = 0; i < existingLines.Count; i++)
                {
                    if (existingLines[i].StartsWith(infoType + ":"))
                    {
                        existingLines[i] = $"{infoType}:{info}";
                        found = true;
                        break;
                    }
                }

                // Add new info if type doesn't exist
                if (!found)
                {
                    existingLines.Add($"{infoType}:{info}");
                }

                // Write all lines back to file
                File.WriteAllLines(memoryFilePath, existingLines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error storing information: {ex.Message}");
            }
        }

        // Recalls stored user information
        public string RecallUserInfo(string userName)
        {
            try
            {
                if (File.Exists(memoryFilePath))
                {
                    string[] lines = File.ReadAllLines(memoryFilePath);
                    List<string> interests = new List<string>();

                    // Process each line in memory file
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("name:"))
                        {
                            continue; // Skip name field
                        }
                        else if (line.StartsWith("interest:"))
                        {
                            // Collect interests
                            interests.Add(line.Substring("interest:".Length));
                        }
                    }

                    // Return personalized message if interests found
                    if (interests.Count > 0)
                    {
                        return $"I remember you're interested in {string.Join(" and ", interests)}. Would you like to know more about these topics?";
                    }
                }
                return "I don't have any specific information stored about your interests yet.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error recalling information: {ex.Message}");
                return "I'm having trouble accessing my memory right now.";
            }
        }
    }
}
