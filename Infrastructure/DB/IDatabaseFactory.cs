namespace Detrack.Infrastructure.DB
{
	public interface IDatabaseFactory
	{
		IDatabase NewRecoverDatabase();
		IDatabase NewConfigDatabase();
		IDatabase NewProtectDatabase();
	}
}