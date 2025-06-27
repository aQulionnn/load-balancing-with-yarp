using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication(BearerTokenDefaults.AuthenticationScheme)
    .AddBearerToken();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.Lease.TryGetMetadata("RETRY_AFTER", out var retryAfter);
        TimeSpan retryAfterTime = TimeSpan.Parse(retryAfter.ToString());
        await context.HttpContext.Response.WriteAsync($"Retry again after {retryAfterTime.TotalSeconds} seconds.", cancellationToken); 
    };
    
    options.AddPolicy("customPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 1,
                Window = TimeSpan.FromSeconds(15),
            }
        )
    );
});

builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));
    
    options.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new  OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(builder.Configuration["Keycloak:AuthorizationUrl"]!),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "openid" },
                    { "profile", "profile" },
                }
            }
        }
    });

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Keycloak",
                    Type = ReferenceType.SecurityScheme
                },
                In = ParameterLocation.Header,
                Name = "Bearer",
                Scheme = "Bearer",
            },
            []
        }
    };
    
    options.AddSecurityRequirement(securityRequirement);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("health");

app.UseRateLimiter();

app.MapReverseProxy();

app.Run();