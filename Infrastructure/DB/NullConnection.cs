using System.Data;

namespace Detrack.Infrastructure.DB
{
	public class NullConnection : IDbConnection
	{
		public void Dispose() { }

		public IDbTransaction BeginTransaction()
		{
			return new NullTransaction(this);
		}

		public IDbTransaction BeginTransaction(IsolationLevel il)
		{
			return new NullTransaction(this);
		}

		public void Close() { }

		public void ChangeDatabase(string databaseName) { }

		public IDbCommand CreateCommand()
		{
			return new NullCommand();
		}

		public void Open() { }

		public string ConnectionString { get; set; }

		public int ConnectionTimeout
		{
			get { return -1; }
		}

		public string Database
		{
			get { return string.Empty; }
		}

		public ConnectionState State
		{
			get { return ConnectionState.Closed; }
		}
	}
}