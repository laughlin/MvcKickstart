using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using ServiceStack.Text;
using Spruce;
using Spruce.Migrations;
using Spruce.Schema;
using StructureMap;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof($rootnamespace$.MvcKickstartDbMigrator), "PreStart", Order = 1000)]

namespace $rootnamespace$ 
{
	// Migrates the db for mvckickstart assemblies
	public static class MvcKickstartDbMigrator
	{
		// Get a list of all IMigration classes
		public static IList<Type> GetMigrationTypes()
		{
			return AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.StartsWith("MvcKickstart"))
				.SelectMany(asm => asm.GetTypes().Where(t => typeof (IMigration).IsAssignableFrom(t)))
				.ToList();
		}

		public static void PreStart() 
		{
			var db = ObjectFactory.GetInstance<IDbConnection>();
			if (!db.TableExists(db.GetTableName<DataMigration>()))
			{
				db.CreateTable<DataMigration>();
			}
			var alreadyExecutedMigrations = db.Query<DataMigration>("select * from [{0}]".Fmt(db.GetTableName<DataMigration>()));
			var migrationTypes = GetMigrationTypes();
			foreach (var migration in migrationTypes.Where(t => !alreadyExecutedMigrations.Any(m => m.Name == t.Name)).Select(x => (IMigration)Activator.CreateInstance(x)).OrderBy(x => x.Order))
			{
				migration.Execute(db);
				// add migration to database
				var migrationData = new DataMigration
					                    {
						                    CreatedOn = DateTime.UtcNow,
						                    Name = migration.GetType().Name
					                    };
				db.Save(migrationData);
			}
		}
	}
}
