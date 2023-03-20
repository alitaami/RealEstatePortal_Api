using System.Net.Mime;
using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Entities.Base;
using Common.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;

namespace WebApiCourse.WebFramework.Middlewares;


public static class APIExceptionHandlerMiddlewareExtensions
{
    // it is for  ==> to simplize using this handler in program.cs

    public static void UseNAPExceptionHandlerMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<APIExceptionHandlerMiddleware>();
    }
}
public class APIExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostingEnvironment _env;
    private readonly ILogger<APIExceptionHandlerMiddleware> _logger;

    public APIExceptionHandlerMiddleware(RequestDelegate next
        , IHostingEnvironment env
        , ILogger<APIExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _env = env;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string message = "";
        HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
        ErrorCodeEnum apiStatusCode = ErrorCodeEnum.InternalError;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
  
            var sqlEx = ex.InnerException as SqlException;
 
            /// unique constraint violation
            if (sqlEx != null && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                message = Resource.SqlUpdateException;
                apiStatusCode = ErrorCodeEnum.UpdateError;
                httpStatusCode = HttpStatusCode.InternalServerError;
            }
            /// Foreign key constraint violation
            if (sqlEx != null && (sqlEx.Number == 547))
            {
                message = Resource.SqlForeignKeyException;
                apiStatusCode = ErrorCodeEnum.ForeignKeyVonstraintViolation;
                httpStatusCode = HttpStatusCode.InternalServerError;
            }
            else
            {
                // when it runs in Development version, it shows complete errors to developer 

                if (_env.IsDevelopment())
                {
                    var dic = new Dictionary<string, string>
                    {
                        ["Exception"] = ex.Message,
                        ["StackTrace"] = ex.StackTrace,
                    };
                    if (ex.InnerException != null)
                    {

                        dic.Add("InnerException.Exception", ex.InnerException.Message);
                        dic.Add("InnerException.StackTrace", ex.InnerException.StackTrace);
                    }
                    message = JsonConvert.SerializeObject(dic);
                }
                else
                {
                    message = ex.Message;
                }
            }
         
            await WriteToResponseAsync();
        }
        async Task WriteToResponseAsync()
        {
            if (context.Response.HasStarted)
                throw new InvalidOperationException(Resource.InvalidOperationException);

            var response = context.Response;
            response.ContentType = MediaTypeNames.Application.Json;

            var apiResult = new ApiResult(httpStatusCode, apiStatusCode, message, null);
            var result = JsonConvert.SerializeObject(apiResult);

            response.StatusCode = (int)httpStatusCode;

            await response.WriteAsync(result);

        }
        //void SetUnAuthorizeResponse(Exception exception)
        //{
        //    var mes = message;

        //    if (_env.IsDevelopment())
        //    {
        //        var dic = new Dictionary<string, string>
        //        {
        //            ["Exception"] = exception.Message,
        //            ["StackTrace"] = exception.StackTrace
        //        };
        //        if (exception is SecurityTokenExpiredException tokenException)
        //            dic.Add("Expires", tokenException.Expires.ToString());

        //        message = JsonConvert.SerializeObject(dic);
        //    }
        //    else
        //    {
        //        message = mes;
        //    }
        //}
    }
}
