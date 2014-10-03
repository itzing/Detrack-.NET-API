using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Detrack.Infrastructure.DB
{
	public class DatabaseRow
	{
		private readonly Dictionary<string, object> fields;

		public DatabaseRow() : this(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)) { }

		public DatabaseRow(Dictionary<string, object> fields)
		{
			this.fields = fields;
		}

		public object this[string field]
		{
			get
			{
				try
				{
					return fields[field];
				}
				catch (KeyNotFoundException e)
				{
					throw new DataException(string.Format("Field [{0}] could not be found. Please check version of your database.", field), e);
				}
			}
		}

		public T As<T>(string field)
		{
			if (string.IsNullOrEmpty(field))
				throw new ArgumentNullException("field");

			return CastTo<T>(this[field]);
		}

		public static T CastTo<T>(object obj)
		{
			if (obj == null || obj == DBNull.Value)
			{
				return default(T);
			}

			try
			{
				return (T)obj;
			}
			catch (InvalidCastException) { }

			Type t = typeof(T);

			try
			{
				if (t.IsEnum)
				{
					Type ut = Enum.GetUnderlyingType(t);

					if (ut != typeof(string))
						return (T)Enum.ToObject(t, Convert.ChangeType(obj, ut));

					return (T)Enum.Parse(t, obj.ToString());
				}

				//TODO: System.InvalidCastException : Cannot cast [09/24/2013 14:49:03] to type [DateTime].
				//When local machine has specific to the region culture settings

				if (t == typeof(DateTime))
					return (T)((object)(DateTime.Parse(obj.ToString(), CultureInfo.InvariantCulture)));

				return (T)Convert.ChangeType(obj, t);
			}
			catch (Exception e)
			{
				if (obj.Equals(""))
					return default(T);

				throw new InvalidCastException(string.Format("Cannot cast [{0}] to type [{1}].", obj, t.Name), e);
			}
		}

		public bool IsNull(string fieldName)
		{
			if (string.IsNullOrEmpty(fieldName))
				throw new ArgumentNullException("fieldName");

			object value;
			fields.TryGetValue(fieldName, out value);

			if (value == null)
				return true;

			return value == DBNull.Value;
		}

		public void Add(string field, object value)
		{
			if (string.IsNullOrEmpty(field))
				throw new ArgumentNullException("field");

			if (fields.ContainsKey(field))
				throw new DataException(string.Format("Duplicated field '{0}'", field));

			fields.Add(field, value);
		}

		public Dictionary<string, object> Fields
		{
			get { return fields; }
		}

		public bool HasField(string field)
		{
			if (string.IsNullOrEmpty(field))
				return false;

			return fields.Keys.Any(key => key.Equals(field, StringComparison.OrdinalIgnoreCase));
		}
	}
}