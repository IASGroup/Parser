using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace BussinessLogic;

public static class Dependencies
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }
}