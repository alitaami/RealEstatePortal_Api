using Carter;
using EstateAgentApi.MinimalApi;
using Microsoft.Extensions.DependencyInjection;

namespace EstateAgentApi
{
    public static class ApiServiceExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            // Register your Carter module and other services here
            services.AddCarter();
            services.AddSingleton<Home_MinimalApi>();  // Register your minimal API module

            // Add any other services specific to your API layer
            return services;
        }
    }
}
