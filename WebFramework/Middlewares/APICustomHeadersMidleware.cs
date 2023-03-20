using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace WebApiCourse.WebFramework.Middlewares;

public static class APICustomHeadersMidlewareExtensions
{
    // it is for  ==> to simplize using this handler in program.cs
    public static void UseNAPCustomHeadersMidleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<APICustomHeadersMidleware>();
    }
}
public class APICustomHeadersMidleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly CustomHeadersToAddAndRemove _deniedHeaders;

    public APICustomHeadersMidleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        //_deniedHeaders = _configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        //foreach (var headerValuePair in _headers.HeadersToAdd)
        //{
        //    context.Response.Headers[headerValuePair.Key] = headerValuePair.Value;
        //}

        //foreach (var header in _headers.HeadersToRemove)
        //{
        //    context.Response.Headers.Remove(header);
        //}

        await _next(context);
        var test = context.Response.Headers["X-Powered-By"];

        //context.Response.Headers.Remove("X-Powered-By");
    }
}

public class CustomHeadersToAddAndRemove
{
    public Dictionary<string, string> HeadersToAdd = new();
    public HashSet<string> HeadersToRemove = new();
}
