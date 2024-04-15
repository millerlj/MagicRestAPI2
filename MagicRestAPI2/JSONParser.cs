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
        private object lockObject = new object();
        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

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
            await semaphore.WaitAsync();
            {
                try 
                {
                    string filePath = $"Data/{filename}.json";
                    using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                    await File.WriteAllTextAsync($"Data/{filename}.json", JsonSerializer.Serialize(_content.ToList()));

                    //await JsonSerializer.SerializeAsync(fileStream, _content);
                    Console.WriteLine($"Wrote {filename}.json");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            semaphore.Release();
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
                content.ManaCost == null);
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

        #region finding parameters in card db

        public List<string> GetAllCardNames()
        {
            return _content.Where(c => c.Name != null).Select(c => c.Name).ToList();
        }

        public List<string> GetAllUniqueCardTypes()
        {
            return _content.Select(c => c.TypeLine).Distinct().ToList();
        }

        public List<string> GetAllCreatureTypes()
        {
            return _content.Where(c => c.TypeLine.Contains("Creature")).Select(c => c.TypeLine).Distinct().ToList();
        }

        public List<HelperCard> GetPowerAndToughness()
        {
            return _content.Where(c => c.TypeLine.Contains("Creature")).Select(c => new HelperCard { Name = c.Name, Type = c.TypeLine, Power = ConvertStringToInt(c.Power), Toughness = ConvertStringToInt(c.Toughness) }).ToList();
        }

        public List<string> GetLikelyhoodOfPowerAndToughness()
        {
            var (powers, toughnesses) = AllCreaturesPowerAndToughness();
            var powerGroups = powers.GroupBy(p => p).Select(pg => new { Key = pg.Key, Count = pg.Count() });
            var toughnessGroups = toughnesses.GroupBy(t => t).Select(tg => new { Key = tg.Key, Count = tg.Count() });
            List<string> returnable = new List<string>();
            foreach (var group in powerGroups)
            {
                returnable.Add($"Power: {group.Key} Count: {group.Count}");
            }
            foreach (var group in toughnessGroups)
            {
                returnable.Add($"Toughness: {group.Key} Count: {group.Count}");
            }
            Console.WriteLine(returnable.ToString()); 
            return returnable;  
        }

        private (List<int>, List<int>) AllCreaturesPowerAndToughness()
        {
            List<int> powers = new List<int>();
            List<int> toughnesses = new List<int>();
            var creatures = _content.Where(c => c.TypeLine.Contains("Creature"));
            powers.AddRange(creatures.Select(c => ConvertStringToInt(c.Power)));
            toughnesses.AddRange(creatures.Select(c => ConvertStringToInt(c.Toughness)));
            return (powers, toughnesses);
        }

        private int ConvertStringToInt(string s)
        {
            if (s == "*")
            {
                return -1;
            }
            return int.Parse(s);
        }

        // To help convey the power and toughness of cards, since its a pair of numbers
        public class HelperCard
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public int Power { get; set; }
            public int Toughness { get; set; }
        }
        #endregion finding parameters in card db
    }
}
