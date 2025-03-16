using Asp.Versioning;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using webapi.Features.FeatureFlags;

namespace webapi.Controllers.v2;

[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class MessageController : ControllerBase
{
    [HttpGet]
    [FeatureGate(MessageFeatureFlags.EnableGetMessageV2)]
    public IActionResult Get()
    {
        var url = Request.GetDisplayUrl();
        return Ok(new { Url = url, Date = DateTime.Now.ToShortDateString() });
    }
}