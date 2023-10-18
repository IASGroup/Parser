namespace DataAccess;

public interface IDbInitializer
{
    void Initialize(AppDbContext context);
}