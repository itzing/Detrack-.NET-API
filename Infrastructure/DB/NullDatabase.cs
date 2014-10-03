using System;
using System.Collections.Generic;
using System.Data;
using Detrack.Infrastructure.Logging;

namespace Detrack.Infrastructure.DB
{
	internal class NullDatabase : IDatabase
	{
		public DatabaseRow ExecuteOne(string sql, params object[] parameters)
		{
			return null;
		}

		public DatabaseRow ExecuteOne(IDbCommand command, params object[] parameters)
		{
			return null;
		}

		public DatabaseRow ExecuteOne(string tableName, string pkName, object pkValue)
		{
			return new DatabaseRow();
		}

		public long ExecuteCount(string tableName, string pkName, object pkValue)
		{
			return 0;
		}

		public long ExecuteCount(string tableName)
		{
			return 0;
		}

		public T GetRowID<T>(string tableName)
		{
			return default(T);
		}

		public int ExecuteNonQuery(string sql)
		{
			return 0;
		}

		public int ExecuteNonQuery(string sql, params object[] parameters)
		{
			return 0;
		}

		public int ExecuteNonQuery(string[] sqlStrings)
		{
			return 0;
		}

		public List<DatabaseRow> ExecuteQuery(string sql)
		{
			return new List<DatabaseRow>();
		}

		public List<DatabaseRow> ExecuteQuery(string sql, params object[] parameters)
		{
			return new List<DatabaseRow>();
		}

		public DatabaseRow ExecuteQueryOne(string sql, params object[] parameters)
		{
			return null;
		}

		public IEnumerable<DatabaseRow> ExecuteReader(string sql, params object[] parameters)
		{
			return new DatabaseRow[0];
		}

		public IEnumerable<DatabaseRow> ExecuteReader(IDbCommand command, params object[] parameters)
		{
			return new DatabaseRow[0];
		}

		public T ExecuteScalar<T>(string sql)
		{
			return default(T);
		}

		public T ExecuteScalar<T>(string sql, params object[] parameters)
		{
			return default(T);
		}

		public T ExecuteScalar<T>(IDbCommand cmd, params object[] parameters)
		{
			return default(T);
		}

		public int ExecuteNonQuery(IDbCommand command, params object[] parameters)
		{
			return 0;
		}

		public IDbConnection NewConnection()
		{
			return new NullConnection();
		}

		public bool ColumnExists(string tableName, string columnName)
		{
			return false;
		}

		public bool CheckConsistency()
		{
			return true;
		}

		public void InsertRows(IEnumerable<DatabaseRow> databaseRows, string table)
		{
			throw new NotImplementedException();
		}

		public ILog Log { get; set; }

		class NullBulk : IDisposable
		{
			public void Dispose() { }
		}
	}
}