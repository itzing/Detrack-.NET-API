using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Detrack.Infrastructure.DB
{
	public class SqlDatabaseAdapter : IDatabaseAdapter
	{
		private readonly string connectionString;

		public SqlDatabaseAdapter()
		{
			connectionString = ConfigurationManager.ConnectionStrings["Detrack"].ToString();
		}

		public IDbConnection Connection()
		{
			return new SqlConnection(connectionString);
		}

		public IDataParameter Parameter(string name, object value)
		{
			//if (value is int)
			//	return new SqlParameter { ParameterName = name, Value = value, SqlDbType = SqlDbType.Int};

			return new SqlParameter { ParameterName = name, Value = value ?? DBNull.Value };
		}
	}
}