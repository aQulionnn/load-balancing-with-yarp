using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MessageController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var url = Request.GetDisplayUrl();
        return Ok(new { Url = url });
    }
}