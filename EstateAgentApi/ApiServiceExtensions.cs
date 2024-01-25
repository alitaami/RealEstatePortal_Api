using Carter;
using EstateAgentApi.MinimalApi;
using System.Reflection;

namespace EstateAgentApi
{
    public static class ApiServiceExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            // Register your Carter module and other services here
            services.AddCarter();
            services.AddTransient<Home_MinimalApi>();

            // Add any other services specific to your API layer

            return services;
        }
    }
}
