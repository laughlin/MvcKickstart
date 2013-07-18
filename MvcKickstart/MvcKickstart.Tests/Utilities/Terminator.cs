using System.Data;
using System.Linq;
using Dapper;
using KickstartTemplate.Models.Users;
using Spruce;

namespace KickstartTemplate.Tests.Utilities
{
	public class Terminator
	{
		protected IDbConnection Db { get; set; }
		public Terminator(IDbConnection db)
		{
			Db = db;
		}
		public void ClearSqlData(params string[] tables)
		{
			if (Db == null) 
				return;

			var sql = tables.Aggregate("", (current, table) => current + string.Format("if OBJECT_ID('{0}') is not null delete from [{0}];", table));
			Db.Execute(sql);
		}

		public void ClearUsers()
		{
			ClearSqlData(ClearUsersTables());
		}
		private string[] ClearUsersTables()
		{
			return new []
				{
					Db.GetTableName<PasswordRetrieval>(),
					Db.GetTableName<User>()
				};
		}
	}
}
