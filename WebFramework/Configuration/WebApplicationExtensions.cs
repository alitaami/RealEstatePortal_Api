using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiCourse.WebFramework.Middlewares;
using WebFramework.Configuration.Swagger;

namespace WebFramework.Configuration
{
    public static class WebApplicationExtensions
    {
        public static WebApplication Configure(this WebApplication app)
        {
            var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();

            try
            {
                app.UseNAPCustomHeadersMidleware();
                app.UseNAPExceptionHandlerMiddleware();
                app.UseHttpsRedirection();
                app.UseHsts();
                app.UseCors();
                app.UseStaticFiles();
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllers();

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwaggerAndUI();

                }
                return app;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }
    }

}
