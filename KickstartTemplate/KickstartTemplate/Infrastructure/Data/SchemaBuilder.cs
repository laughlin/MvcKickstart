using System.Data;
using Dapper;
using KickstartTemplate.Infrastructure.Data.Schema;
using Spruce.Schema;

namespace KickstartTemplate.Infrastructure.Data
{
	public class SchemaBuilder
	{
		private readonly ISchemaSection[] _sections =
			{
				new GlobalSection(),
				new UserSection()
			};

		protected IDbConnection Db { get; set; }
		public SchemaBuilder(IDbConnection db)
		{
			Db = db;
		}

		public void GenerateSchema(bool dropExisting)
		{
			Db.DropAllScriptedObjects();
			
			if (dropExisting)
				Db.DropAllTables();

			foreach (var section in _sections)
			{
				foreach (var table in section.Tables)
				{
					Db.CreateTable(table);
				}
			}
			foreach (var section in _sections)
			{
				foreach (var obj in section.ScriptedObjects)
				{
					Db.Execute(obj.CreateScript);
				}
			}
		}
	}
}