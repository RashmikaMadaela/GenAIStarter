

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
