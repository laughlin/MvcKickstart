using System.Data.SqlClient;
using MvcKickstart.Infrastructure.Data;
using StructureMap;

namespace MvcKickstart
{
	public class DbConfig
	{
		public static void Bootstrap()
		{
			using (var db = ObjectFactory.GetInstance<SqlConnection>())
			{
				db.Open();			
				DataMigrator.RunMigrations(db);
			}
		}
	}
}