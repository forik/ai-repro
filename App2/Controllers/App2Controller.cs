using System.Threading.Tasks;
using App2;
using Microsoft.AspNetCore.Mvc;

namespace App3.Controllers
{
    [ApiController]
    [Route("")]
    public class App2Controller : ControllerBase
    {
        [HttpPost("ping")]
        public async Task<IActionResult> Ping([FromBody] object message, [FromServices] App3Client client)
        {
            await client.SendAsync(message.ToString());
            return Ok();
        }
    }
}