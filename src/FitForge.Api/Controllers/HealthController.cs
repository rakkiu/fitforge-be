using Microsoft.AspNetCore.Mvc;

namespace FitForge.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class HealthController : ControllerBase
{
    [HttpGet("health")]
    [HttpGet("health/ready")]
    public IActionResult GetHealth()
    {
        return Ok(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow.ToString("O"),
            service = "FitForge.Api",
            version = "1.0.0"
        });
    }

    [HttpGet("health/live")]
    public IActionResult GetLiveness()
    {
        return Ok(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow.ToString("O")
        });
    }
}
