using System.Text.Json;
using System;
using MagicParser;

namespace MagicRestAPI
{
    public class FileDownloader
    {        

        public static async Task<List<Card>> DownloadFileAsync(string uri, Func<string, List<string>> CardTypeGroupBy, bool SaveOnlyEnglishCards)
        {
            var timeoutSignal = new CancellationTokenSource(TimeSpan.FromMinutes(20));

            HttpClientStreamService srv = new(uri);
            var card_file = await srv.Execute();
            Console.WriteLine($"File Downloader: Downloaded {card_file.Count} cards");
            
            // Uncomment this next line will cause some sort of buffer overflow
            //await File.WriteAllTextAsync("Data/master_card_file.json", JsonSerializer.Serialize(card_file));

            var LanguageGroups = card_file.GroupBy(c => c.Language).Select(cf => new { Key = cf.Key, card = cf });
            Console.WriteLine("File Downloader: " + LanguageGroups.Count() + " languages found");
            
            if (SaveOnlyEnglishCards)
            {
                LanguageGroups = LanguageGroups.Where(lang => lang.Key == "en");
                Console.WriteLine("File Downloader: Discarding non-english cards");
            }
            
            foreach(var LangGroup in LanguageGroups)
            {
                await File.WriteAllTextAsync($"Data/{LangGroup.Key}_card_file.json", JsonSerializer.Serialize(LangGroup.card));
                Console.WriteLine($"File Downloader: Wrote {LangGroup.Key}_card_file.json");
                
                var values = LangGroup.card.GroupBy(c => CardTypeGroupBy(c.TypeLine).ToString());
                Console.WriteLine($"File Downloader: {values.Count()} type groups found");
                
                foreach (var typeGroups in values)
                {
                    await File.AppendAllTextAsync($"Data/{LangGroup.Key}_{typeGroups.Key}_card_file.json",
                    JsonSerializer.Serialize(typeGroups, new JsonSerializerOptions() { WriteIndented = false }));
                    Console.WriteLine($"File Downloader: Wrote {LangGroup.Key}_{typeGroups.Key}_card_file.json");
                }
            }
            return card_file;
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

            private async Task<List<Card>> GetCardsWithStream()
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
