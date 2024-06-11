using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ToDoListWebApp.Contexts;
using ToDoListWebApp.Entities;

namespace ToDoListWebApp.Helpers;
public static class ConfigHelper
{
    public static void ConfigureDbContext(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TodoContext>(options =>
            options.UseSqlite("Data Source=TodoItems.db;"));
    }

    public static void ConfigureIdentity(IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<TodoContext>()
            .AddDefaultTokenProviders();
    }
}
