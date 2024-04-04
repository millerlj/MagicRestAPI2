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
        private List<Card> _content = new();

        public ReadOnlyCollection<Card> Content
        {
            get { return _content.AsReadOnly(); }
        }

        public JsonParser()
        {            
        }

        #region loading and saving json files
        public async Task DownloadFiles(bool EnglishOnly)
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
            _content = await FileDownloader.DownloadFileAsync(uri, ToCardType, true);
        }

        public async Task LoadFileAsync(string filename)
        {
            string filePath = $"Data/{filename}.json";
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            _content = await JsonSerializer.DeserializeAsync<List<Card>>(fileStream);
            Console.WriteLine($"Loaded {_content.Count} cards from {filePath}");
        }

        public async Task SaveFileAsync(string filename)
        {
            string filePath = $"Data/{filename}.json";
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            //await File.WriteAllTextAsync($"Data/{filename}.json", JsonSerializer.Serialize(_content.ToList()));
            
            await JsonSerializer.SerializeAsync(fileStream, _content);
            Console.WriteLine($"Wrote {filename}.json");
        }

        public IEnumerable<string?> ListJsonFiles()
        {
            string[] jsonFiles = Directory.GetFiles("Data/.", "*.json");
            return jsonFiles.Select(Path.GetFileName);
        }

        public bool DoesFileExist(string filePath)
        {
            return File.Exists(filePath);
        }
        #endregion loading and saving json files

        #region card db manipulation methods
        public bool FilterContentByTypeLine(string[] include, string[] exclude)
        {
            List<Card> cards = new List<Card>();
            try
            {
                var Groups = _content.GroupBy(c => string.Join(",", ToCardType(c.TypeLine))).Select(cf => new { Key = cf.Key, card = cf });
                var filteredGroups = Groups.Where(g =>
                {
                    var cardTypes = g.Key.Split(",");
                    if (exclude != null && cardTypes.Intersect(exclude).Any())
                    {
                        return false;
                    }
                    if (include != null && !cardTypes.Intersect(include).Any())
                    {
                        return true;
                    }
                    return false;
                });
                cards.AddRange(filteredGroups.SelectMany(g => g.card));
                _content = cards;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public bool RemoveReprints()
        {
            List<Card> cards = new List<Card>();
            try
            {
                var Groups = _content.GroupBy(c => c.Reprint).Select(cf => new { Key = cf.Key, card = cf });
                var Group = Groups.Where(g => g.Key == false);
                cards.AddRange(Group.SelectMany(g => g.card));
                _content = cards;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public void PruneNullContent()
        {
            _content = _content.Where(c => !IsContentNull(c)).ToList();
        }

        private bool IsContentNull(Card content)
        {
            return (content.Name == null &&
                content.TypeLine == null &&
                content.OracleText == null &&
                content.ManaCost == null) ? true : false;
        }

        static List<string> ToCardType(string typeline)
        {
            if (typeline == null)
            {
                return new List<string>();
            }
            List<string> types = typeline.Split(" ").ToList();
            List<string> returnable = new List<string>();
            foreach (var type in types)
            {
                string NormalizedType = type.Normalize();
                if (NormalizedType == "Legendary".Normalize())
                {
                    returnable.Add("Legendary");
                }
                if (NormalizedType == "Creature".Normalize())
                {
                    returnable.Add("Creature");
                }
                if (NormalizedType == "Artifact".Normalize())
                {
                    returnable.Add("Artifact");
                }
                if (NormalizedType == "Enchantment".Normalize())
                {
                    returnable.Add("Enchantment");
                }
                if (NormalizedType == "Instant".Normalize())
                {
                    returnable.Add("Instant");
                }
                if (NormalizedType == "Sorcery".Normalize())
                {
                    returnable.Add("Sorcery");
                }
                if (NormalizedType == "Land".Normalize())
                {
                    returnable.Add("Land");
                }
                if (NormalizedType == "Planeswalker".Normalize())
                {
                    returnable.Add("Planeswalker");
                }
            }   
            return returnable;
        }

        public bool CompareCardTypes(string[] firstCardTypes, string[] secondCardTypes)
        {
            foreach (var type in firstCardTypes)
            {
                if (!secondCardTypes.Contains(type))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion card db manipulation methods
    }
}
