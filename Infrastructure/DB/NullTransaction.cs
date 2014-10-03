using System.Data;

namespace Detrack.Infrastructure.DB
{
	public class NullTransaction : IDbTransaction
	{
		public NullTransaction(IDbConnection connection)
		{
			Connection = connection;
		}

		public void Dispose()
		{
		}

		public void Commit()
		{
			Commited = true;
		}

		public bool Commited
		{
			get;
			private set;
		}

		public void Rollback()
		{
			Rolledback = true;
		}

		public bool Rolledback { get; private set; }

		public IDbConnection Connection { get; private set; }
		public IsolationLevel IsolationLevel { get; private set; }
	}
}