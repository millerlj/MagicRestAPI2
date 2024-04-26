using MagicParser;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MagicRestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MagicController : ControllerBase
    {
        JsonParser _parser;

        public MagicController(JsonParser parser)
        {
            this._parser = parser;
        }

        [HttpGet("stats")]
        public string GetStats()
        {
            return $"Number of cards: {_parser.Content.Count}";
        }

        [HttpGet("save_and_download_file/{filename}")]
        public async Task<IActionResult> SaveAndDownloadFile(string filename)
        {
            string filePath = $"Data/{filename}.json";
            if (!_parser.DoesFileExist(filePath))
            {
                return NotFound("File not found");
            }
            MemoryStream memoryStream = new MemoryStream();

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                await fileStream.CopyToAsync(memoryStream);
            }

            memoryStream.Position = 0;

            return File(memoryStream, "application/json", Path.GetFileName(filePath));
        }

        [HttpGet("list_json_files")]
        public IEnumerable<string?> ListJsonFiles()
        {
            return _parser.ListJsonFiles();
        }

        [HttpGet("load_file/{file_name}")]
        public async Task LoadFile(string file_name)
        {
            string path = Path.GetFullPath(file_name);
            await _parser.LoadFileAsync(file_name);
            Console.WriteLine("create new parser");
        }

        [HttpPost("save_file/{file_name}")]
        public async Task SaveFile(string file_name)
        {
            await _parser.SaveFileAsync(file_name);
        }

        [HttpPost("filter")]
        public IActionResult FilterByCardType([FromBody] ApiCardTypeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.IncludeTypes == null && request.ExcludeTypes == null)
            {
                return BadRequest("IncludeTypes and ExcludeTypes cannot both be null");
            }

            bool success = _parser.FilterContentByTypeLine(request.IncludeTypes, request.ExcludeTypes);
            if (!success)
            {
                return BadRequest("Exception during Filter, Complain to Luke");
            }
            return Ok();
        }

        [HttpPost("all_card_names")]
        public IEnumerable<string> GetAllCardNames()
        {
            return _parser.GetAllCardNames();
        }

        [HttpPost("all_unique_major_card_types")]
        public IEnumerable<string> GetAllUniqueCardTypes()
        {
            List<string> card_types = _parser.GetAllUniqueCardTypes();
            
            HashSet<string> unique = new HashSet<string>();

            foreach (string type in card_types)
            {

                string[] BreakUp = type.Split(" ");
                for (int i = 0; i < BreakUp.Length; i++)
                {
                    if (BreakUp[i] != "—")
                    {
                        unique.Add(BreakUp[i]);
                    }   
                    else
                    {
                        break;
                    }
                }
                
            }
            return unique;
        }

        [HttpPost("all_oracle_text/{separate_clause}/{separate_cause_effect}")]
        public IActionResult GetAllOracleText(bool separate_clause, bool separate_cause_effect)
        {
            if (!separate_cause_effect && !separate_clause)
            {
                return Ok(_parser.GetAllOracleText(false, false));
            } 
            else if (separate_clause && !separate_cause_effect)
            {
                return Ok(_parser.GetAllOracleText(true, false));
            }
            else if (separate_clause && separate_cause_effect)
            {
                return Ok(_parser.GetAllOracleText(true, true));
            }
            else if (!separate_clause && separate_cause_effect)
            {
                return BadRequest("Have to separate based on clauses before separate cause and effect");
            }
            else
            {
                return BadRequest("Dafuq, these are invalid parameters");
            }
        }

        [HttpPost("all_oracle_text_by_card_type/{types}")]
        public IEnumerable<string> GetAllOracleTextByCardType(string types)
        {
            string[] strings = types.Split(",");
            return _parser.GetAllOracleTextByCardType(strings);
        }

        [HttpPost("all_unique_minor_card_types/{oftype}")]
        public IEnumerable<string> GetAllUniqueCardTypes(string oftype)
        {
            List<string> card_types = _parser.GetAllUniqueCardTypes();

            HashSet<string> unique = new HashSet<string>();

            foreach (string type in card_types)
            {
                if (type.Contains(oftype))
                {
                    string[] BreakUp = type.Split(" ");
                    bool start = false;

                    for (int i = 0; i < BreakUp.Length; i++)
                    {
                        if (BreakUp[i] == "—")
                        {
                            start = true;
                        }

                        if (start)
                        {
                            unique.Add(BreakUp[i]);
                        }
                    }
                }

            }
            return unique;
        }

        [HttpPost("power_and_toughness/{RangeOrRandomPair}")]
        public IActionResult GetPowerAndToughness(string RangeOrRandomPair)
        {
            if (RangeOrRandomPair.ToLower() == "range")
            {
                List<JsonParser.HelperCard> cards = _parser.GetPowerAndToughness();
                List<JsonParser.HelperCard> unique = cards.Select(x => x).Distinct().ToList();
                List<int> Power = unique.Select(x => x.Power).Distinct().ToList();
                List<int> Toughness = unique.Select(x => x.Toughness).Distinct().ToList();
                return Ok((Power, Toughness));
            }
            else if (RangeOrRandomPair.ToLower() == "random")
            {
                return Ok(_parser.GetLikelyhoodOfPowerAndToughness());
            }
            else
            {
                return BadRequest("Invalid parameter, must be 'range' or 'random'");
            }
        }



        [HttpPost("remove_reprints")]
        public IActionResult RemoveReprints()
        {
            _parser.RemoveReprints();
            return Ok();
        }

        [HttpPost("prune")]
        public void PruneCardCollection()
        {
            _parser.PruneNullContent();
        }

        [HttpPost("downloadfile/{EnglishOnly}")]
        public async Task DownloadFiles(bool EnglishOnly)
        {
            try
            {
                await _parser.DownloadFiles(EnglishOnly);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        public class ApiRequest
        {
            [Required]
            public string? Field { get; set; }
            public string[]? Parameters { get; set; }
        }

        public class ApiCardTypeRequest
        {
            public string[]? IncludeTypes { get; set; }
            public string[]? ExcludeTypes { get; set; }
        }
    }
}
    


        
