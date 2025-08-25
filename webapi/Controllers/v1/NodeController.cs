using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using webapi.LeaderElection.Bully;

namespace webapi.Controllers.v1;

[ApiVersion("1.0", Deprecated = false)]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class NodeController(Node node) 
    : ControllerBase
{
    private readonly Node _node = node;

    [HttpPost]
    [Route("heartbeat")]
    public IActionResult Send()
    {
        _node.LastHeartbeat = DateTime.UtcNow;
        
        return Ok(new
        {
            NodeId = _node.Id
        });
    }
}