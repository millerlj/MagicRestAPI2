using System.Text.Json;
using System;
using MagicParser;

namespace MagicRestAPI
{
    public class FileDownloader
    {
        public static async Task DownloadFileAsync(string uri)
        {
            var timeoutSignal = new CancellationTokenSource(TimeSpan.FromMinutes(20));

            HttpClientStreamService srv = new(uri);
            var card_file = await srv.Execute();

            List<Card> cards = JsonSerializer.Deserialize<List<Card>>(File.ReadAllText("master_card_file.json"));

            await File.WriteAllTextAsync("Data/master_card_file.json", JsonSerializer.Serialize(card_file, new JsonSerializerOptions() { WriteIndented = true }));

            cards.GroupBy(c => c.Lang == "en").Select(g => g.ToList()).ToList().ForEach(g => JsonSerializer.Serialize(g, new JsonSerializerOptions() { WriteIndented = true }));

            await File.WriteAllTextAsync("Data/master_card_file_en.json", JsonSerializer.Serialize(cards, new JsonSerializerOptions() { WriteIndented = true }));
        }

        private class HttpClientStreamService
        {
            private static readonly HttpClient _httpClient = new HttpClient();
            private readonly JsonSerializerOptions _options;

            public HttpClientStreamService(string uri)
            {
                _httpClient.BaseAddress = new Uri(uri);
                _httpClient.Timeout = new TimeSpan(0, 0, 30);
                _httpClient.DefaultRequestHeaders.Clear();

                _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            }

            public Task<List<Card>> Execute()
            {
                return GetCardsWithStream();
            }

            private async Task<List<Card>?> GetCardsWithStream()
            {
                using (var response = await _httpClient.GetAsync("all-cards/all-cards-20240325212428.json", HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    var stream = await response.Content.ReadAsStreamAsync();

                    return await JsonSerializer.DeserializeAsync<List<Card>?>(stream, _options);
                }
            }
        }
    }
}
