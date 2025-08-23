using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace webapi.Encryption;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SecureHttpAttribute : Attribute, IFilterFactory
{
    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var config =  serviceProvider.GetRequiredService<IOptions<SystemSettings>>();
        return new SecureHttpFilter(config.Value);
    }

    public bool IsReusable => false;
}