using System.Reflection;

namespace DataAccess;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        var dbInitializerInterface = typeof(IDbInitializer);
        var initializers = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.IsClass && dbInitializerInterface.IsAssignableFrom(x));
        foreach (var initializer in initializers)
        {
            var instance = (IDbInitializer)Activator.CreateInstance(initializer)!;
            instance.Initialize(context);
        }
    }
}