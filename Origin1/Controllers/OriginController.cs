using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Origin1.Controllers
{
    [ApiController]
    [Route("")]
    public class OriginController : ControllerBase
    {
        [HttpPost("rest")]
        public async Task<IActionResult> Rest([FromBody] object message, [FromServices] App2Client client)
        {
            await client.SendAsync(message.ToString());
            return Ok();
        }
    }
}