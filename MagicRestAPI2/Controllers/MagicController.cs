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
