using Newtonsoft.Json;
using System.Net.Http.Headers;

using RootMobile.Constants;

namespace RootMobile.Services
{
    public class MessageEdge
    {
        public string role { get; set; } // "user" або "assistant"
        public string content { get; set; }
    }


    public class OpenAIService : IOpenAIService
    {

        private readonly HttpClient _httpClient;
        private const string SupabaseFunctionUrl = "https://bgrmftzpmzwahkxooxxf.supabase.co/functions/v1/openai_request";
        private const string SupabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImJncm1mdHpwbXp3YWhreG9veHhmIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MjgwNjQxNDQsImV4cCI6MjA0MzY0MDE0NH0.tpt8AvNusdWAETsViz0UlKr7AhBvVCWfdFPPBtlF3bE";


        private readonly List<MessageEdge> messageHistory = new();

        public OpenAIService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseAnonKey}");
        }

        public async Task<string> AskQuestion(string prompt)
        {
            try
            {
                messageHistory.Add(new MessageEdge { role = "user", content = prompt });

                var requestData = new { messages = messageHistory };
                var json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(SupabaseFunctionUrl, content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseString);

                string assistantReply = result.choices[0].message.content.ToString();
                messageHistory.Add(new MessageEdge { role = "assistant", content = assistantReply });

                return assistantReply;
            }
            catch (Exception ex)
            {
                return $"Помилка: {ex.Message}";
            }
        }

        public void ClearHistory()
        {
            messageHistory.Clear();
        }

        public async Task<bool> ContainsProfanity(string text)
        {
            var prompt = $"Чи містить цей текст матюки або образливі слова? Відповідай лише 'так' або 'ні': \"{text}\"";

            var response = await AskQuestion(prompt);
            return response.Trim().ToLower().Contains("так");
        }

    }
}
