using System.Collections.Generic;
using System.Data;
using Detrack.Infrastructure.Logging;

namespace Detrack.Infrastructure.DB
{
	public interface IDatabase
	{
		ILog Log { get; set; }

		IDbConnection NewConnection();

		long ExecuteCount(string tableName, string pkName, object pkValue);
		long ExecuteCount(string tableName);

		int ExecuteNonQuery(string sql, params object[] parameters);
		int ExecuteNonQuery(IDbCommand command, params object[] parameters);
		int ExecuteNonQuery(string[] sqlStrings);

		List<DatabaseRow> ExecuteQuery(string sql, params object[] parameters);
		DatabaseRow ExecuteQueryOne(string sql, params object[] parameters);
		DatabaseRow ExecuteOne(IDbCommand command, params object[] parameters);
		DatabaseRow ExecuteOne(string tableName, string pkName, object pkValue);

		IEnumerable<DatabaseRow> ExecuteReader(string sql, params object[] parameters);
		IEnumerable<DatabaseRow> ExecuteReader(IDbCommand command, params object[] parameters);

		T ExecuteScalar<T>(string sql, params object[] parameters);
		T ExecuteScalar<T>(IDbCommand cmd, params object[] parameters);
	}
}