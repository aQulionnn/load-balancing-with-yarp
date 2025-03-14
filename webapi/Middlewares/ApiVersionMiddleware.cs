using Asp.Versioning.ApiExplorer;

namespace webapi.Middlewares;

public class ApiVersionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext httpContext, IApiVersionDescriptionProvider provider)
    {
        var requestedVersion = httpContext.GetRequestedApiVersion();
        var isDeprecated = provider.ApiVersionDescriptions.
            Any(v => v.ApiVersion == requestedVersion && v.IsDeprecated);

        if (isDeprecated)
        {
            var minSupportedVersions = provider.ApiVersionDescriptions
                .Where(v => !v.IsDeprecated)
                .OrderBy(v => v.ApiVersion)
                .FirstOrDefault()?.ApiVersion.ToString();  
            
            if (minSupportedVersions != null)
                httpContext.Response.Headers.Append("api-minimal-supported-version", minSupportedVersions);
        }
        
        await _next(httpContext);
    }
}