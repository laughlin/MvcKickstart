using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using KickstartTemplate.Infrastructure.Data.Migrations;
using ServiceStack.Text;
using Spruce;
using Spruce.Migrations;
using Spruce.Schema;

namespace KickstartTemplate.Infrastructure.Data
{
	public class DataMigrator
	{
		// get a list of all IMigration classes
		public static IEnumerable<Type> GetMigrationTypes()
		{
			var type = typeof(DataMigrator);
			return type.Assembly.GetTypes().Where(t => typeof (IMigration).IsAssignableFrom(t)).ToList();
		}

		/// <summary>
		/// Run all defined data migrations that have not been run yet
		/// </summary>
		/// <param name="db">current database connection</param>
		/// <returns>Count of migrations run</returns>
		public static int RunMigrations(IDbConnection db)
		{
			var migrationsRun = 0;
			var migrationTypes = GetMigrationTypes();

			if (!db.TableExists(db.GetTableName<DataMigration>()))
			{
				// Create all data, run data init migration
				var schemaBuilder = new SchemaBuilder(db);
				schemaBuilder.GenerateSchema(false);

				var dataInitMigration = new InitializeDbMigration();
				dataInitMigration.Execute(db);

				// Populate all migrations as run
				var migrations = migrationTypes.Select(x => new DataMigration
					{
						Name = x.Name
					});
				return db.BulkInsert(migrations);
			}

			var alreadyExecutedMigrations = db.Query<DataMigration>("select * from [{0}]".Fmt(db.GetTableName<DataMigration>()));
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
				migrationsRun++;
			}

			return migrationsRun;
		}
	}}