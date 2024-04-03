using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicParser
{
    internal class JsonParser
    {
        private JArray _jsonContent;
        private string _currentFilePath;

        public JArray JsonContent
        {
            get { return _jsonContent; }
        }

        public JsonParser()
        {
            _jsonContent = new JArray();
        }

        public JsonParser(string filePath)
        {
            _jsonContent = new JArray();
            _currentFilePath = filePath;
            loadJsonAsync(filePath).Wait();
        }
        
        #region loading and saving json files
        
        public void loadNewFile(string filePath)
        {
            _currentFilePath = filePath;
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
            using (StreamReader file = File.OpenText(filePath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                _jsonContent = (JArray) await JToken.ReadFromAsync(reader);
            }

            // Need to Serialize the JSON object to get the correct format
            //_jsonContent = JArray.Parse(JsonConvert.SerializeObject(_jsonContent));
            foreach (var card in _jsonContent["data"])
            {
                Console.WriteLine(card["name"]);
            }
            Console.WriteLine(JsonPopulated());
            Console.WriteLine(_jsonContent.First().GetType());
            Console.WriteLine(_jsonContent.First().ToString());
        }

        public bool SaveCurrentJson(string filePath)
        {
            try
            {
                File.WriteAllText(filePath, _jsonContent.ToString());
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        #endregion loading and saving json files

        #region card query methods
        public IEnumerable<string> GetAllCardsWithType(string type)
        {
            List<string> types = new List<string>();
            types = type.Split(' ').ToList();
            types.RemoveAll(s => s.Contains("-"));
            
            if (_jsonContent.Count == 0)
            {
                return new List<string>();
            }

            List<string> cards = new List<string>();
            foreach (var card in _jsonContent["data"])
            {
                foreach (var cardType in card["type_line"].ToString().Split(' '))
                {
                    if (types.Contains(cardType))
                    {
                        cards.Add(card["name"].ToString());
                        break;
                    }
                }
            }
            return cards;
        }

        public HashSet<string> GetAllCardTypes()
        {
            HashSet<string> set = new HashSet<string>();
            
            foreach (var card in _jsonContent["data"])
            {
                foreach (string cardType in card["type_line"].ToString().Split(' '))
                {
                    set.Add(cardType);
                }
            }

            return set;
        }

        public int JsonPopulated()
        {
            return _jsonContent.Count;
        }
        #endregion card query methods

        #region card db manipulation methods

        public async Task<bool> FilterJson(string field_type, string[] include, string[] exclude)
        {
            if (_jsonContent.Count == 0)
            {
                return false;
            }

            JArray data = (JArray) _jsonContent["data"];
            JArray newData = new JArray();

            foreach (var card in data)
            {
                if (include.Length > 0)
                {
                    if (include.Contains(card[field_type].ToString()))
                    {
                        newData.Add(card);
                    }
                }
                else if (exclude.Length > 0)
                {
                    if (!exclude.Contains(card[field_type].ToString()))
                    {
                        newData.Add(card);
                    }
                }
            }

            _jsonContent["data"] = newData;
            return true;
        }

        #endregion card db manipulation methods
       
    }
}
