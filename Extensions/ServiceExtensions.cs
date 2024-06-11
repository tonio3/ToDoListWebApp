using ToDoListWebApp.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ToDoListWebApp.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            ConfigHelper.ConfigureDbContext(services, configuration);
            ConfigHelper.ConfigureIdentity(services);
            AuthenticationHelper.ConfigureAuthentication(services, configuration);
            SwaggerHelper.ConfigureSwagger(services);
        }
    }
}
