using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

namespace webapi.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/chat")]
[ApiController]
public class ChatController(IChatClient chatClient) : ControllerBase
{
    private readonly IChatClient _chatClient = chatClient;

    [HttpPost]
    public async Task<IActionResult> Post(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();

        var message = new ChatMessage(ChatRole.User, "What's in this image?");
        message.Contents.Add(new DataContent(fileBytes, file.ContentType));

        var response = await _chatClient.GetResponseAsync(message);
    
        return Ok(response.Text);
    }
}