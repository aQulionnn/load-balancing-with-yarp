using Asp.Versioning;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using webapi.Features.FeatureFlags;

namespace webapi.Controllers.v1;

[ApiVersion("1.0", Deprecated = true)]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class MessageController : ControllerBase
{
    [HttpGet]
    [FeatureGate(MessageFeatureFlags.EnableGetMessageV1)]
    public IActionResult Get()
    {
        var url = Request.GetDisplayUrl();
        return Ok(new { Url = url });
    }
}