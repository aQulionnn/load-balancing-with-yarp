namespace apigateway.Authentication;

public class ApiKeyEndpointFilter(IConfiguration configuration) 
    : IEndpointFilter
{
    private readonly IConfiguration _configuration = configuration;
    
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey))
        {
            return TypedResults.Unauthorized();
        }
        
        var apiKey = _configuration.GetValue<string>(AuthConstants.ApiKeySectionName);
        if (!apiKey.Equals(extractedApiKey))
        {
            return TypedResults.Unauthorized();
        }
        
        return await next(context);
    }
}