namespace Migrations.Initializers;

public interface IDbInitializer
{
	void Initialize(AppDbContext context);
}