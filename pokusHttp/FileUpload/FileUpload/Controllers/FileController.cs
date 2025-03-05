using FileUpload.Models;
using Microsoft.AspNetCore.Mvc;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FileUpload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        ConnectionMultiplexer redis;
        IDatabase db;
        public FileController() { 
        redis = ConnectionMultiplexer.Connect("file.database:6379");
        db = redis.GetDatabase();
        }

        // GET: api/<FileController>
        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            var fileContent = await db.ListLeftPopAsync("QueueForFiles");
            if (fileContent.IsNullOrEmpty)
            {
                return NotFound("No file found in the queue.");
            }
            return Ok((string)fileContent);
        }


        // GET api/<FileController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<FileController>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] fileUploadRequest file)
        {
            //if file was empty, the readToEnd got stuck
            if (file.file == null || file.file.Length == 0)
            {
                return BadRequest("File is empty or not provided.");
            }

            using (StreamReader reader = new StreamReader(file.file.OpenReadStream()))
            {
                var fileContent = await reader.ReadToEndAsync();
                await db.ListRightPushAsync("QueueForFiles", fileContent);
                //I want to know if post worked
                return Ok(fileContent);
            }
        }

        // PUT api/<FileController>/5
        [HttpPut("{id}")]
        public void Put([FromBody] IFormFile file)
        {

        }

        // DELETE api/<FileController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {

        }
    }
}
