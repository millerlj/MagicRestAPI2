using System.Text.Json;
using System;
using MagicParser;

namespace MagicRestAPI
{
    public class FileDownloader
    {        

        public static async Task<List<Card>> DownloadFileAsync(string uri, Func<string,string> grouper)
        {
            var timeoutSignal = new CancellationTokenSource(TimeSpan.FromMinutes(20));

            HttpClientStreamService srv = new(uri);
            var card_file = await srv.Execute();

         //   await File.WriteAllTextAsync("Data/master_card_file.json", JsonSerializer.Serialize(card_file));

            var langs = card_file.GroupBy(c => c.Language).Select(cf => new { Key = cf.Key, card = cf });
            
            foreach(var lan in langs)
            {
                await File.WriteAllTextAsync($"Data/{lan.Key}_card_file.json", JsonSerializer.Serialize(lan.card));
                var values = lan.card.GroupBy(c => grouper(c.TypeLine));
                foreach (var typeGroups in values)
                {
                    await File.AppendAllTextAsync($"Data/{lan.Key}_{typeGroups.Key}_card_file.json",
                        JsonSerializer.Serialize(typeGroups, new JsonSerializerOptions() { WriteIndented = true }));
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
