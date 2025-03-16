using Microsoft.FeatureManagement.FeatureFilters;

namespace webapi.Features.Contexts;

public class UserTargetingContext(IHttpContextAccessor httpContextAccessor) : ITargetingContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private const string CacheKey = "UserTargetingContext.TargetingContext";

    public ValueTask<TargetingContext> GetContextAsync()
    {
        HttpContext httpContext = _httpContextAccessor.HttpContext!;
        
        if (httpContext.Items.TryGetValue(CacheKey, out object? value))
            return new ValueTask<TargetingContext>((TargetingContext)value!);
        
        var targetingContext = new TargetingContext
        {
            UserId = GetUserId(httpContext),
            Groups = GetUserGroups(httpContext)
        };
        
        return new ValueTask<TargetingContext>(targetingContext);
    }

    private static string GetUserId(HttpContext? httpContext)
    {
        return httpContext?.Request.Headers["X-User-Id"].FirstOrDefault() ?? string.Empty;
    }

    private static string[] GetUserGroups(HttpContext? httpContext)
    {
        var userGroups = httpContext?.Request
            .Headers["X-User-Groups"]
            .FirstOrDefault()?
            .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];
        
        return userGroups;
    }
}