using Microsoft.AspNetCore.Mvc;

namespace WebApplicationFirewall.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeatureActivatorController : ControllerBase
    {
        private readonly IFeatureActivatorService _settingsService;

        public FeatureActivatorController(IFeatureActivatorService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet("feature")]
        public IActionResult GetSettings()
        {
            return Ok(_settingsService);
        }

        [HttpPost("feature")]
        public IActionResult UpdateSettings([FromBody] Settings settings)
        {
            _settingsService.UpdateSettings(settings);

            return Ok(new { message = "Settings updated successfully." });
        }
    }
}
