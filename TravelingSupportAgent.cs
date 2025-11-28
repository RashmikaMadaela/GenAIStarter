/*
 * Assessment Task: Multi-turn Traveling Support Agent Chatbot
 * 
 * Showcase your achievement on LinkedIn, GitHub, or your portfolio to impress future employers and peers!
 * 
 * You are required to build a multi-turn chatbot that provides traveling support assistance using the GPT-4o model 
 * via GitHub's AI inference API. Your chatbot should maintain conversation context, offer meaningful travel help, 
 * and support various travel scenarios and destinations.
 * 
 * What to Do:
 * 
 * 1. Environment Setup
 *    - Load environment variables from .env file
 *    - Use your GitHub token from API_TOKEN environment variable
 *    - Ensure your .env is listed in .gitignore
 * 
 * 2. API Initialization  
 *    - Use the Azure.AI.Inference package (already included)
 *    - Configure the API client to use your GitHub token and the GitHub models endpoint
 * 
 * 3. Conversation Logic
 *    - Implement a loop that lets the user and bot exchange messages
 *    - Store the conversation history so the chatbot remembers previous messages
 *    - Allow the user to exit the chat gracefully (e.g., by typing 'exit')
 * 
 * 4. Traveling Support Features
 *    - Help users plan trips and itineraries
 *    - Provide destination recommendations and travel tips
 *    - Assist with booking guidance (flights, hotels, activities)
 *    - Offer cultural information and local customs advice
 *    - Help with travel documentation and visa requirements
 *    - Provide weather information and packing suggestions
 *    - Handle travel emergencies and problem-solving
 *    - Support multiple destinations and travel styles (budget, luxury, adventure, etc.)
 * 
 * 5. Error Handling
 *    - Handle API errors and invalid inputs gracefully
 *    - Provide helpful error messages to users
 * 
 * Example Interactions:
 * 
 * User: "I want to plan a 7-day trip to Japan in spring"
 * Agent: "That sounds wonderful! Spring is cherry blossom season in Japan. I can help you plan an amazing 7-day itinerary. 
 *         What's your budget range and what type of experiences interest you most - cultural sites, food, nature, or modern attractions?"
 * 
 * User: "I'm having trouble with my flight booking, it got cancelled"
 * Agent: "I'm sorry to hear about your cancelled flight. Let me help you with the next steps. First, contact your airline 
 *         for rebooking options. Are you traveling soon? I can also suggest alternative flights and help you understand your rights."
 * 
 * Where to Implement:
 * - Complete the implementation in this file (TravelingSupportAgent.cs)
 * - Follow the existing patterns from other examples in this project
 * - Use the same project structure and dependencies
 * 
 * Running Your Solution:
 * dotnet run --project TravelingSupportAgent.csproj
 * 
 * Complete the assessment as described below to earn your certificate and badge!
 * 
 * Once you have finished implementing your multi-turn traveling support agent chatbot and submitted your pull request, 
 * you will be eligible to receive an official certificate and badge.
 * 
 * How to claim your certificate and badge:
 * 1. Complete all steps in the "Assessment Task" section above
 * 2. Test your implementation thoroughly
 * 3. Submit your solution following the submission guidelines
 * 4. After your submission is reviewed and approved, you will receive your personalized certificate and badge via email
 * 
 * Submission Guidelines:
 * 1. Ensure your code follows the existing project patterns
 * 2. Test your implementation with various travel scenarios
 * 3. Make sure error handling works properly
 * 4. Document any additional features you've implemented
 * 5. Create a pull request with your completed solution
 * 
 * Tips for Success:
 * - Study the existing examples (MultiTurnChat.cs, CodingAssistant.cs) for patterns
 * - Focus on creating a helpful and knowledgeable travel assistant
 * - Consider edge cases and error scenarios
 * - Make the conversation feel natural and engaging
 * - Test with various travel-related queries
 * 
 * Good luck with your implementation!
 */

using Azure;
using Azure.AI.Inference;

namespace GenAIStarter
{
    /// <summary>
    /// Traveling Support Agent - Multi-turn travel assistance chatbot
    /// Run with: dotnet run --project TravelingSupportAgent.csproj
    /// </summary>
    public class TravelingSupportAgent
    {
        public static async Task Main(string[] args)
        {
            
            Console.WriteLine("=== Traveling Support Agent ===");
            Console.WriteLine("Hi I'm Your Personal Travel Assistant!");
            Console.WriteLine("\nI can help you with planning trips, booking guidance, destination recommendations, travel tips, and more.");
            Console.WriteLine("\nType 'exit' to quit the chat at any time.\n");
            

            LoadEnvFile();
            
            var token = Environment.GetEnvironmentVariable("API_TOKEN");
            var endpointUrl = Environment.GetEnvironmentVariable("API_ENDPOINT");
            
            if (string.IsNullOrWhiteSpace(token))
            {
                Console.Error.WriteLine("Error: API_TOKEN not found!");
                Console.WriteLine("Please create a .env file with your API configuration:");
                Console.WriteLine("1. Copy .env.example to .env");
                Console.WriteLine("2. Add your API token and endpoint to the .env file");
                return;
            }

            if (string.IsNullOrWhiteSpace(endpointUrl))
            {
                Console.Error.WriteLine("Error: API_ENDPOINT not found!");
                Console.WriteLine("Please add API_ENDPOINT to your .env file");
                return;
            }

            try
            {
                // Initialize the client
                var endpoint = new Uri(endpointUrl);
                var client = new ChatCompletionsClient(endpoint, new AzureKeyCredential(token));
                var model = "gpt-4o";

                // Initialize conversation with system message
                var messages = new List<ChatRequestMessage>
                {
                    new ChatRequestSystemMessage(@"You are an expert travel assistant. Help users with following:
                        - Help users plan trips and itineraries
                        - Provide destination recommendations and travel tips
                        - Assist with booking guidance (flights, hotels, activities)
                        - Offer cultural information and local customs advice
                        - Help with travel documentation and visa requirements
                        - Provide weather information and packing suggestions
                        - Handle travel emergencies and problem-solving
                        - Support multiple destinations and travel styles (budget, luxury, adventure, etc.)
                        - Provide clear, practical advice when helpful and ask follow up questions when needed.
                        And make sure the conversation feel natural and engaging")
                };


                Console.WriteLine("Let's plan your next adventure! Ask me anything...\n");

                // Conversation loop
                while (true)
                {
                    Console.Write("You: ");
                    var input = Console.ReadLine();

                    if (string.IsNullOrEmpty(input) || input.Trim().ToLower() == "exit")
                    {
                        Console.WriteLine("Goodbye!");
                        break;
                    }

                    // Add user message to conversation history
                    messages.Add(new ChatRequestUserMessage(input));

                    // Create request with full conversation history
                    var requestOptions = new ChatCompletionsOptions { Model = model };
                    foreach (var msg in messages)
                        requestOptions.Messages.Add(msg);

                    // Get AI response
                    var response = await client.CompleteAsync(requestOptions);
                    var reply = response.Value.Content;
                    
                    Console.WriteLine($"Travel Assistant: {reply}\n");
                    
                    // Add assistant response to conversation history
                    messages.Add(new ChatRequestAssistantMessage(reply));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }

        private static void LoadEnvFile()
        {
            try
            {
                if (File.Exists(".env"))
                {
                    foreach (var line in File.ReadAllLines(".env"))
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                            continue;

                        var parts = line.Split('=', 2);
                        if (parts.Length == 2)
                        {
                            Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not load .env file: {ex.Message}");
            }
        }

    }

}
