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
        public async Task<ActionResult<object>> Get()
        {
            var fileId = await db.ListLeftPopAsync("filesQueueDone");

            if (fileId.IsNullOrEmpty)
            {
                return NotFound("file queue is empty");
            }
            var fileContent = await db.StringGetAsync($"fileContent:{fileId}");
            if (fileContent.IsNullOrEmpty)
            {
                return NotFound($"file content for : {fileId} doesnt exist");
            }
            var userName = await db.HashGetAsync($"fileMetadata:{fileId}", "userName");
            await db.KeyDeleteAsync($"fileContent:{fileId}");
            await db.KeyDeleteAsync($"fileMetadata:{fileId}");
            return Ok(new{fileId = fileId.ToString(),userName = userName.ToString(),content = fileContent.ToString()});
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
            //creates unique value I will use as id
            string fileId = Guid.NewGuid().ToString();

            //order of files to transformation
            await db.ListRightPushAsync("filesQueueReady", fileId);

            await db.HashSetAsync($"fileMetadata:{fileId}", new HashEntry[]
            {
                new HashEntry("userName", file.userName),
                new HashEntry("uploadFormat", file.uploadFormat),
                new HashEntry("wantedFormat", file.wantedFormat)
            });

            using (StreamReader reader = new StreamReader(file.file.OpenReadStream()))
            {
                var fileContent = await reader.ReadToEndAsync();
                await db.StringSetAsync($"fileContent:{fileId}", fileContent);
                //I want to know if post worked
                return Ok(new { fileId, fileContent });
            }
        }
    }
}
