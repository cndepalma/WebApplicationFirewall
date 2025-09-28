using Microsoft.AspNetCore.Mvc;
using WebApplicationFirewall.Helpers;

namespace WebApplicationFirewall.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InternetProtocolController : ControllerBase
    {
        private readonly IStorageHelper _storageHelper = new StorageHelper();
        public InternetProtocolController(IStorageHelper storageHelper)
        {
            _storageHelper = storageHelper;
        }

        [HttpGet("ips")]
        public IActionResult GetIps()
        {
            var ips = _storageHelper.LoadIps();
            return Ok(ips);
        }

        [HttpPost("ips/add")]
        public IActionResult AddIp([FromBody] string ip)
        {
            var ips = _storageHelper.LoadIps();
            if (!ips.Contains(ip))
            {
                ips.Add(ip);
                _storageHelper.SaveIps(ips);
                return Ok(new { message = "IP added successfully." });
            }
            return BadRequest(new { message = "IP already exists." });
        }

        [HttpPost("ips/remove")]
        public IActionResult RemoveIp([FromBody] string ip)
        {
            var ips = _storageHelper.LoadIps();
            if (ips.Contains(ip))
            {
                ips.Remove(ip);
                _storageHelper.SaveIps(ips);
                return Ok(new { message = "IP removed successfully." });
            }
            return NotFound(new { message = "IP not found." });
        }
    }
}
