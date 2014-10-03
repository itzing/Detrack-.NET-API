using System.Data;

namespace Detrack.Infrastructure.DB
{
	internal class NullCommand : IDbCommand
	{
		public void Dispose() { }

		public void Prepare() { }

		public void Cancel() { }

		public IDbDataParameter CreateParameter()
		{
			return null;
		}

		public int ExecuteNonQuery()
		{
			return -1;
		}

		public IDataReader ExecuteReader()
		{
			throw new System.NotSupportedException();
		}

		public IDataReader ExecuteReader(CommandBehavior behavior)
		{
			throw new System.NotSupportedException();
		}

		public object ExecuteScalar()
		{
			throw new System.NotSupportedException();
		}

		public IDbConnection Connection { get; set; }

		public IDbTransaction Transaction { get; set; }

		public string CommandText { get; set; }

		public int CommandTimeout { get; set; }

		public CommandType CommandType { get; set; }

		public IDataParameterCollection Parameters
		{
			get { throw new System.NotSupportedException(); }
		}

		public UpdateRowSource UpdatedRowSource { get; set; }
	}
}