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
        JsonParser parser;


        public MagicController(JsonParser parser)
        {
            this.parser = parser;
        }

        [HttpGet("list_json_files")]
        public IEnumerable<string?> ListJsonFiles()
        {                        
            return parser.ListJsonFiles();
        }

        [HttpGet("load_file/{file_name}")]
        public bool LoadFile(string file_name)
        {
            string path = Path.GetFullPath(file_name);
            parser.loadNewFile(file_name);
            Console.WriteLine("create new parser");
            return true;
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

        [HttpPost("downloadfile")]
        public async Task DownloadFiles()
        {
            try
            {
                parser.DownloadFiles().Wait();
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
