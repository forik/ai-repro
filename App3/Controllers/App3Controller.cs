using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace App3.Controllers
{
    [ApiController]
    [Route("")]
    public class App3Controller : ControllerBase
    {
        [HttpPost("ping")]
        public async Task<IActionResult> Ping([FromBody] object message, [FromServices] ILogger<App3Controller> logger)
        {
            logger.LogInformation(message.ToString());
            return Ok();
        }
    }
}