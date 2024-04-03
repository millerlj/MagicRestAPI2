using MagicRestAPI;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MagicParser
{
    public class JsonParser
    {
        private List<Card> _content = new() ;

        public ReadOnlyCollection<Card> Content
        {
            get { return _content.AsReadOnly(); }
        }

        public JsonParser()
        {            
        }

        public async Task DownloadFiles()
        {
            string uri = "https://data.scryfall.io/";
            if (Directory.Exists("Data"))
            {
                Directory.Delete("Data", true);
                Directory.CreateDirectory("Data");
            }
            else
            {
                Directory.CreateDirectory("Data");
            }
            await FileDownloader.DownloadFileAsync(uri, ToType);
        }

        #region loading and saving json files

        public void loadNewFile(string filePath)
        {            
            loadJsonAsync(filePath).Wait();
        }

        public IEnumerable<string?> ListJsonFiles()
        {
            string[] jsonFiles = Directory.GetFiles(".", "*.json");
            return jsonFiles.Select(Path.GetFileName);
        }

        public bool DoesFileExist(string filePath)
        {
            return File.Exists(filePath);
        }

        private async Task loadJsonAsync(string filePath)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            _content = await JsonSerializer.DeserializeAsync<List<Card>>(fileStream);
        }
        #endregion loading and saving json files

        #region card query methods      

     
        #endregion card query methods

        #region card db manipulation methods

        public bool FilterJson(string field_type, string[] include, string[] exclude)
        {
            
            return true;
        }

        #endregion card db manipulation methods


        static string ToType(string typeline)
        {
            if (typeline?.Contains("Creature") == true)
            {
                return "Creature";
            }
            if (typeline?.Contains("Artifact") == true)
            {
                return "Artifact";
            }

            return "General";
        }
    }
}
