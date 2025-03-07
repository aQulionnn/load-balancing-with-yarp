using Asp.Versioning;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers.v1;

[ApiVersion("1.0", Deprecated = true)]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class MessageController : ControllerBase
{
    [HttpGet]
    [MapToApiVersion(1)]
    public IActionResult Get()
    {
        var url = Request.GetDisplayUrl();
        return Ok(new { Url = url });
    }
}