using Asp.Versioning;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers.v2;

[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class MessageController : ControllerBase
{
    [HttpGet]
    [MapToApiVersion(2)]
    public IActionResult Get()
    {
        var url = Request.GetDisplayUrl();
        return Ok(new { Url = url, Date = DateTime.Now.ToShortDateString() });
    }
}