using Asp.Versioning;
using Microsoft.Extensions.AI;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using webapi.Features.Contexts;
using webapi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "V1", Version = "v1.0" });
    options.SwaggerDoc("v2", new OpenApiInfo { Title = "V2", Version = "v2.0" });
});

builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
        options.ReportApiVersions = false;
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddFeatureManagement()
    .WithTargeting<UserTargetingContext>();

builder.Services.AddSingleton<IChatClient>
    (new OllamaChatClient(new Uri("http://localhost:11434"), "llava:7b"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "webapi v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "webapi v2");
    });
}

app.UseMiddleware<ApiVersionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();