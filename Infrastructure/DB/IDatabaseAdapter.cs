using System.Data;

namespace Detrack.Infrastructure.DB
{
	public interface IDatabaseAdapter
	{
		IDbConnection Connection();
		IDataParameter Parameter(string name, object value);
	}
}