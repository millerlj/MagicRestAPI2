using MagicParser;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MagicRestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MagicController : ControllerBase
    {
        JSONParser parser;

        [HttpGet("list_json_files")]
        public IEnumerable<string?> ListJsonFiles()
        {
            if (parser == null)
            {
                parser = new JSONParser();
            }
            
            return parser.ListJsonFiles();
        }

        [HttpGet("load_file/{file_name}")]
        public bool LoadFile(string file_name)
        {
            string path = Path.GetFullPath(file_name);

            if (parser == null)
            {
                parser = new JSONParser(file_name);
                Console.WriteLine("create new parser");
                return true;
            }
            else if (parser.DoesFileExist(path))
            {
                parser.loadNewFile(file_name);
                Console.WriteLine("load new file");
                return true;
            }
            return false;
        }

        [HttpGet("card_type/{type}")]
        public IEnumerable<string> GetCardType(string type)
        {
            if (parser == null)
            {
                return null;
            }

            List<string> types = new List<string>();
            types = type.Split(' ').ToList();

            List<string> cards = new List<string>();

            foreach (string t in types)
            {
                cards.AddRange(parser.GetAllCardsWithType(t).Except(cards));
            }

            return cards;
        }

        [HttpGet("filter/{inclusion_val}/{type}")]
        public IEnumerable<string>? FilterByCardType(string inclusion_val, string type)
        {
            if (inclusion_val != "exclude" || inclusion_val != "include")
            {
                return null;
            }

            List<string> types = new List<string>();
            types = type.Split(' ').ToList();
            types.RemoveAll(s => s.Contains("-"));

            List<string> cards = new List<string>();

            foreach (string t in types)
            {
                //cards.AddRange(parser.FilterCards(inclusion_val, t).Except(cards));
            }

            return cards;
        }

        [HttpGet("save_current_json/{filename}")]
        public bool SaveCurrentJson(string filename)
        {
            if (parser == null)
            {
                return false;
            }
            return parser.SaveCurrentJson(filename);
        }

        [HttpGet("printFirstCard")]
        public string PrintFirstCard()
        {
            if (parser == null)
            {
                return null;
            }
            return parser.PrintFirstCard();
        }

    }
}
