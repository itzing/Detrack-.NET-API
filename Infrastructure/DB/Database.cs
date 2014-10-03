using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using Detrack.Infrastructure.Logging;

namespace Detrack.Infrastructure.DB
{
	public class Database : IDatabase
	{
		private static ILog log = new NullLogger();
		private readonly IDatabaseAdapter adapter;

		public Database(IDatabaseAdapter adapter)
		{
			if (adapter == null)
				throw new ArgumentNullException("adapter");

			this.adapter = adapter;
		}

		#region IDatabase Members

		public ILog Log
		{
			get { return log; }
			set { log = value ?? new NullLogger(); }
		}

		public int ExecuteNonQuery(IDbCommand command, params object[] parameters)
		{
			if (command == null)
				throw new ArgumentNullException("command");

			try
			{
				if (parameters != null)
				{
					command.Parameters.Clear();

					AddParameters(parameters, command);
				}

				return command.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				log.Error(e, FormatErrorMessage(command.CommandText, parameters));
				throw;
			}
		}

		public IDbConnection NewConnection()
		{
			return adapter.Connection();
		}

		public DatabaseRow ExecuteQueryOne(string sql, params object[] parameters)
		{
			List<DatabaseRow> rows = ExecuteQueryImpl(sql, true, parameters);
			return rows.Count > 0 ? rows[0] : null;
		}

		public DatabaseRow ExecuteOne(IDbCommand command, params object[] parameters)
		{
			List<DatabaseRow> rows = ExecuteQueryImpl(command, true, parameters);
			return rows.Count > 0 ? rows[0] : null;
		}

		public DatabaseRow ExecuteOne(string tableName, string pkName, object pkValue)
		{
			string sql = string.Format("SELECT * FROM {0} WHERE {1} = ?", tableName, pkName);
			List<DatabaseRow> rows = ExecuteQueryImpl(sql, true, pkValue);
			return rows.Count > 0 ? rows[0] : null;
		}

		public long ExecuteCount(string tableName, string pkName, object pkValue)
		{
			return ExecuteScalar<long>(string.Format("SELECT COUNT(*) FROM {0} WHERE {1} = ?", tableName, pkName), pkValue);
		}

		public long ExecuteCount(string tableName)
		{
			return ExecuteScalar<long>(string.Format("SELECT COUNT(*) FROM {0}", tableName));
		}

		public int ExecuteNonQuery(string[] sqlStrings)
		{
			int i = 0;

			using (IDbConnection connection = NewConnection())
			{
				connection.Open();

				foreach (string t in sqlStrings)
				{
					try
					{
						using (IDbCommand command = connection.CreateCommand())
						{
							command.CommandText = t;
							i = command.ExecuteNonQuery();
						}
					}
					catch (Exception e)
					{
						log.Error(e, @"SQL = [{0}]", t);
					}
				}
			}

			return i;
		}

		public int ExecuteNonQuery(string sql, params object[] parameters)
		{
			try
			{
				using (IDbConnection connection = NewConnection())
				using (IDbCommand command = connection.CreateCommand())
				{
					command.CommandText = sql;

					if (parameters != null)
					{
						AddParameters(parameters, command);
					}

					connection.Open();
					return command.ExecuteNonQuery();
				}
			}
			catch (Exception e)
			{
				log.Error(e, FormatErrorMessage(sql, parameters));
				throw;
			}
		}

		public List<DatabaseRow> ExecuteQuery(string sql, params object[] parameters)
		{
			return ExecuteQueryImpl(sql, false, parameters);
		}

		public IEnumerable<DatabaseRow> ExecuteReader(IDbCommand command, params object[] parameters)
		{
			if (command == null)
				throw new ArgumentNullException("command");

			if (parameters != null && parameters.Length != 0)
			{
				command.Parameters.Clear();

				AddParameters(parameters, command);
			}

			using (IDataReader reader = command.ExecuteReader())
			{
				if (reader == null)
					yield break;

				while (reader.Read())
				{
					var fields = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

					for (int i = 0; i < reader.FieldCount; i++)
					{
						object value = reader[i];
						string name = reader.GetName(i);
						fields.Add(name, value);
					}

					yield return new DatabaseRow(fields);
				}
			}
		}

		public T ExecuteScalar<T>(string sql, params object[] parameters)
		{
			using (IDbConnection connection = NewConnection())
			using (IDbCommand command = connection.CreateCommand())
			{
				command.CommandText = sql;

				if (parameters != null)
				{
					AddParameters(parameters, command);
				}

				try
				{
					connection.Open();
					return DatabaseRow.CastTo<T>(command.ExecuteScalar());
				}
				catch (Exception e)
				{
					log.Error(e, FormatErrorMessage(sql, parameters));
					throw;
				}
			}
		}

		private void AddParameters(IEnumerable<object> parameters, IDbCommand command)
		{
			int paramsCount = 1;
			foreach (object value in parameters)
			{
				var parameterName = string.Format("@param{0}", paramsCount);
				var regex = new Regex(Regex.Escape("?"));
				command.CommandText = regex.Replace(command.CommandText, parameterName, 1);
				command.Parameters.Add(adapter.Parameter(parameterName, value));
				paramsCount++;
			}
		}

		public T ExecuteScalar<T>(IDbCommand command, params object[] parameters)
		{
			if (command == null)
				throw new ArgumentNullException("command");

			try
			{
				command.Parameters.Clear();

				if (parameters != null)
				{
					AddParameters(parameters, command);
				}
				return DatabaseRow.CastTo<T>(command.ExecuteScalar());
			}
			catch (Exception e)
			{
				log.Error(e, FormatErrorMessage(command.CommandText, parameters));
				throw;
			}
		}

		public IEnumerable<DatabaseRow> ExecuteReader(string sql, params object[] parameters)
		{
			using (IDbConnection connection = NewConnection())
			{
				using (IDbCommand command = connection.CreateCommand())
				{
					command.CommandText = sql;

					if (parameters != null)
					{
						AddParameters(parameters, command);
					}

					connection.Open();
					using (IDataReader reader = command.ExecuteReader())
					{
						if (reader == null)
							yield break;

						while (reader.Read())
						{
							var fields = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

							for (int i = 0; i < reader.FieldCount; i++)
							{
								object value = reader[i];
								string name = reader.GetName(i);
								fields.Add(name, value);
							}

							yield return new DatabaseRow(fields);
						}
					}
				}
			}
		}

		#endregion

		private List<DatabaseRow> ExecuteQueryImpl(string sql, bool topOne, params object[] parameters)
		{
			try
			{
				using (IDbConnection connection = NewConnection())
				{
					using (IDbCommand command = connection.CreateCommand())
					{
						command.CommandText = sql;
						return ExecuteQueryImpl(command, topOne, parameters);
					}
				}
			}
			catch (Exception e)
			{
				log.Error(e, FormatErrorMessage(sql, parameters));
				throw;
			}

		}

		private List<DatabaseRow> ExecuteQueryImpl(IDbCommand command, bool topOne, params object[] parameters)
		{
			try
			{
				command.Parameters.Clear();

				if (parameters != null)
				{
					AddParameters(parameters, command);
				}

				var result = new List<DatabaseRow>();

				if (command.Connection.State != ConnectionState.Open)
					command.Connection.Open();

				using (IDataReader reader = command.ExecuteReader())
				{
					if (reader == null)
						return result;

					while (reader.Read())
					{
						var fields = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

						for (int i = 0; i < reader.FieldCount; i++)
						{
							object value = reader[i];
							string name = reader.GetName(i);
							fields.Add(name, value);
						}

						result.Add(new DatabaseRow(fields));

						if (topOne)
							return result;
					}
				}

				return result;

			}
			catch (Exception e)
			{
				log.Error(e, FormatErrorMessage(command.CommandText, parameters));
				throw;
			}
		}

		private static string FormatErrorMessage(string sql, object[] parameters)
		{
			var sb = new StringBuilder();
			sb.AppendFormat("SQL = '{0}'. Parameters = ", sql);

			if (parameters == null || parameters.Length == 0)
			{
				sb.Append("NONE");
				return sb.ToString();
			}

			foreach (object param in parameters)
			{
				if (param == null)
					sb.Append("null; ");
				else
					sb.AppendFormat("'{0}'=[{1}]; ", param.GetType().Name, param);
			}

			return sb.ToString();
		}
	}
}