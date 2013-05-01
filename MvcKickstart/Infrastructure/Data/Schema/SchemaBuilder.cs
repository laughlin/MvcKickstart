using System.Data;
using Dapper;
using MvcKickstart.Infrastructure.Data.Schema.Extensions;
using MvcKickstart.Infrastructure.Data.Schema.Sections;

namespace MvcKickstart.Infrastructure.Data.Schema
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
			DropAllScriptedObjects();
			
			if (dropExisting)
				DropAllTables();

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

		public void DropAllTables()
		{
			Db.Execute(
@"
DECLARE @SQL NVARCHAR(MAX)	
 
SELECT
	@SQL = '-- Start '
PRINT 'dropping all foreign keys...'

WHILE @SQL > ''
BEGIN
	SELECT
		@SQL = ''
 
	SELECT
		@SQL = @SQL + 
			CASE
				WHEN DATALENGTH(@SQL) < 7500 THEN
					N'alter table ' + QUOTENAME(schema_name(schema_id)) 
					+ N'.' 
					+ QUOTENAME(OBJECT_NAME(parent_object_id)) 
					+ N' drop constraint ' 
					+ name
					+ CHAR(13) + CHAR(10) --+ 'GO' + CHAR(13) + CHAR(10)
				ELSE
					''
			END
	FROM
		sys.foreign_keys
 
	PRINT @SQL
	EXEC SP_EXECUTESQL @SQL
	PRINT '---8<------------------------------------------------------------------------------------------'
END
 
---8<-------------------------------------------------------------
PRINT 'dropping all tables...'
 
exec sp_msforeachtable 'drop table ?'

PRINT 'all tables dropped.'
PRINT 'All done'");
		}

		private void DropAllScriptedObjects()
		{
			Db.Execute(
@"
PRINT 'dropping all procedures...'
 
 --PROCEDURES
 declare @procName varchar(500)
declare cur cursor
    for select [name] from sys.objects where type = 'p'
open cur

fetch next from cur into @procName
      while @@fetch_status = 0
      begin
            if @procName <> 'DeleteAllProcedures'
                  exec('drop procedure ' + @procName)
                  fetch next from cur into @procName
      end

close cur
deallocate cur

PRINT 'all procedures dropped.'
PRINT 'dropping all views...'

--VIEWS
declare cur cursor
    for select [name] from sys.objects where type = 'v'
open cur

fetch next from cur into @procName
      while @@fetch_status = 0
      begin
                  exec('drop view ' + @procName)
                  fetch next from cur into @procName
      end
close cur
deallocate cur

PRINT 'all views dropped.'
PRINT 'dropping all functions...'

--FUNCTIONS
declare cur cursor
    for select [name] from sys.objects where type in (N'FN', N'IF', N'TF', N'FS', N'FT')
open cur

fetch next from cur into @procName
      while @@fetch_status = 0
      begin
                  exec('drop function ' + @procName)
                  fetch next from cur into @procName
      end

close cur
deallocate cur

PRINT 'all functions dropped.'

PRINT 'All done'");
		}
	}
}