namespace Detrack.Infrastructure.DB
{
	internal class NullDatabaseFactory : IDatabaseFactory
	{
		public IDatabase NewRecoverDatabase()
		{
			return new NullDatabase();
		}

		public IDatabase NewConfigDatabase()
		{
			return new NullDatabase();
		}

		public IDatabase NewProtectDatabase()
		{
			return new NullDatabase();
		}
	}
}