using System.Reflection;

namespace Migrations.Initializers;

public static class DbInitializer
{
	public static void Initialize(AppDbContext context)
	{
		if (context.Database.EnsureCreated())
		{
			var types = Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(x => x.IsAssignableTo(typeof(IDbInitializer)) && x.IsClass)
				.ToList();
			foreach (var type in types)
			{
				((IDbInitializer) Activator.CreateInstance(type)!).Initialize(context);
			}
			context.SaveChanges();
		}
	}
}