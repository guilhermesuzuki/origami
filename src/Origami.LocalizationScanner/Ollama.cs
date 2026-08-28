using System.Net.Http.Json;

namespace Origami.LocalizationScanner
{
    internal static class Ollama
    {
        public static async Task<string> PromptAsync(string message)
        {
            var http = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:11434")
            };

            var request = new
            {
                model = "llama3.2:latest",
                messages = new[]
                {
                    new { role = "user", content = message, }
                },
                stream = false // important unless you handle streaming
            };

            var response = await http.PostAsJsonAsync("/api/chat", request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);
            return result;
        }
    }
}
