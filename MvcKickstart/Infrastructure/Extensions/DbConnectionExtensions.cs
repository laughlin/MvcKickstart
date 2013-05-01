using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Design.PluralizationServices;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using Dapper;
using MvcKickstart.Infrastructure.Data;
using MvcKickstart.Infrastructure.Data.Schema;
using MvcKickstart.Infrastructure.Data.Schema.Extensions;
using ServiceStack.Text;
using StackExchange.Profiling.Data;

namespace MvcKickstart.Infrastructure.Extensions
{
	public static class DbConnectionExtensions
	{
		private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> TypeTableName = new ConcurrentDictionary<RuntimeTypeHandle, string>();
		/// <summary>
		/// Gets the name of the sql table for the specified class
		/// </summary>
		/// <typeparam name="T">Class to get the table name</typeparam>
		/// <param name="db"></param>
		/// <returns></returns>
		public static string GetTableName<T>(this IDbConnection db)
		{
			return db.GetTableName(typeof(T));
		}
		/// <summary>
		/// Gets the name of the sql table for the type
		/// </summary>
		/// <param name="type">Type to get the table name</param>
		/// <param name="db"></param>
		public static string GetTableName(this IDbConnection db, Type type)
		{
			string name;
			if (!TypeTableName.TryGetValue(type.TypeHandle, out name))
			{
				if (type.IsSubclassOf(typeof(ScriptedObject)))
				{
					var scriptedObject = (ScriptedObject) Activator.CreateInstance(type);
					name = scriptedObject.Name;
				}
				else
				{
					//NOTE: This as dynamic trick should be able to handle dapper.contrib's Table-attribute as well as the one in EntityFramework 
					var tableattr = type.GetCustomAttributes(false).SingleOrDefault(attr => attr.GetType().Name == "TableAttribute") as dynamic;
					if (tableattr == null)
					{
						var pluralizer = PluralizationService.CreateService(new CultureInfo("en-US"));
						name = pluralizer.Pluralize(type.Name);
						if (type.IsInterface && name.StartsWith("I"))
							name = name.Substring(1);
					}
					else
					{
						name = tableattr.Name;
					}
				}
				TypeTableName[type.TypeHandle] = name;
			}
			return name;
		}
		/// <summary>
		/// Saves the specified item. New items will have their Id property set accordingly
		/// </summary>
		/// <remarks>Id property should be int or long type</remarks>
		/// <typeparam name="T">Type of item to save</typeparam>
		/// <param name="db"></param>
		/// <param name="item">Item to save</param>
		/// <param name="transaction">Db transaction</param>
		/// <returns></returns>
		public static T Save<T>(this IDbConnection db, T item, IDbTransaction transaction = null) where T : class
		{
			UpdateAuditFields(item);

			var type = item.GetType();
			var columns = db.GetColumns(type);
			var primary = columns.SingleOrDefault(x => x.IsPrimary);
			if (primary == null)
				throw new Exception("Unable to find Id field for type: " + type);
			var isInsert = Equals(primary.GetValue(item), GetDefault(primary.Type));
			if (isInsert)
			{
				db.Insert(item, columns, primary, transaction);
			}
			else
			{
				db.Update(item, columns, transaction);
			}
			return item;
		}
		private static void Insert<T>(this IDbConnection db, T item, IList<Column> allColumns, Column primary, IDbTransaction transaction = null)
		{
			var sql = new StringBuilder();
			sql.AppendFormat("insert into {0} (", db.GetTableName<T>());
			var columns = allColumns.Where(x => !x.IsPrimary).ToList();

			for (var i = 0; i < columns.Count; i++)
			{
				var column = columns.ElementAt(i);
				sql.Append("[");
				sql.Append(column.Name);
				sql.Append("]");
				if (i != columns.Count - 1)
				{
					sql.Append(",");
				}
			}
			sql.AppendLine(")");
			if (primary.Type == typeof(Guid))
			{
				sql.Append("OUTPUT inserted.");
				sql.AppendLine(primary.Name);
			}
			sql.Append("values (");
			for (var i = 0; i < columns.Count; i++)
			{
				var column = columns.ElementAt(i);
				sql.AppendFormat("@{0}", column.Name);
				if (i != columns.Count - 1)
				{
					sql.Append(",");
				}
			}
			sql.AppendLine(")");
			if (primary.Type == typeof(Guid))
			{
				var id = db.Query<Guid>(sql.ToString(), item, transaction).Single();
				primary.SetValue(item, id);
			}
			else
			{
				sql.AppendLine("select cast(SCOPE_IDENTITY() as bigint)");

				var id = db.Query<long>(sql.ToString(), item, transaction).Single();
				if (primary.Type == typeof(int))
				{
					primary.SetValue(item, (int) id);
				}
				else
				{
					primary.SetValue(item, id);
				}

			}
		}
		private static void Update<T>(this IDbConnection db, T item, IList<Column> columns, IDbTransaction transaction = null)
		{
			var sql = new StringBuilder();
			sql.AppendFormat("update [{0}] set ", db.GetTableName<T>());
			var columnsWithoutPrimary = columns.Where(x => !x.IsPrimary).ToList();
			for (var i = 0; i < columnsWithoutPrimary.Count; i++)
			{
				var column = columnsWithoutPrimary.ElementAt(i);
				sql.AppendFormat("[{0}]=@{0}", column.Name);
				if (i != columnsWithoutPrimary.Count - 1)
				{
					sql.Append(",");
				}
			}
			sql.AppendFormat(" where [{0}]=@{0}", columns.First(x => x.IsPrimary).Name);
			db.Execute(sql.ToString(), item, transaction);
		}

		/// <summary>
		/// Deletes the specified item
		/// </summary>
		/// <typeparam name="T">Type of item to delete</typeparam>
		/// <param name="db">Database connection</param>
		/// <param name="item">Item to delete</param>
		/// <param name="transaction">Database transaction</param>
		/// <returns></returns>
		public static int Delete<T>(this IDbConnection db, T item, IDbTransaction transaction = null) where T : class
		{
			var type = item.GetType();
			var columns = db.GetColumns(type);
			var primary = columns.SingleOrDefault(x => x.IsPrimary);
			if (primary == null)
				throw new Exception("Unable to find Id field for type: " + type);
			return db.Execute("delete from [{0}] where {1}=@Id".Fmt(db.GetTableName(type), primary.Name), new
				{
					Id = primary.GetValue(item)
				}, transaction);
		}
		/// <summary>
		/// Deletes items from the database that match the specified item representing the where restrictions
		/// </summary>
		/// <typeparam name="T">Type of item to delete</typeparam>
		/// <param name="db">Database connection</param>
		/// <param name="where">Anonymous object representing where clause</param>
		/// <param name="transaction">Database transaction</param>
		/// <returns></returns>
		public static int Delete<T>(this IDbConnection db, object where, IDbTransaction transaction = null)
		{
			if (where is string)
				throw new Exception("Please use db.Execute() instead");

			var sql = "delete from [{0}]".Fmt(db.GetTableName<T>());
			sql += BuildWhereClauseFromAnonymousObject(where);

			return db.Execute(sql, where, transaction);
		}
		/// <summary>
		/// Deletes an item from the database with the specified id
		/// </summary>
		/// <typeparam name="T">Type of object to delete</typeparam>
		/// <param name="db">Database connection</param>
		/// <param name="id">Id of the object to delete</param>
		/// <param name="transaction">Database transaction</param>
		/// <returns></returns>
		public static int DeleteById<T>(this IDbConnection db, object id, IDbTransaction transaction = null)
		{
			var type = typeof(T);
			var columns = db.GetColumns(type);
			var primary = columns.SingleOrDefault(x => x.IsPrimary);
			if (primary == null)
				throw new Exception("Unable to find Id field for type: " + type);
			return db.Execute("delete from [{0}] where {1}=@Id".Fmt(db.GetTableName(type), primary.Name), new
				{
					id
				}, transaction);
		}
		/// <summary>
		/// Deletes items from the database with the specified ids
		/// </summary>
		/// <typeparam name="T">Type of object to delete</typeparam>
		/// <param name="db">Database connection</param>
		/// <param name="ids">Ids of the objects to delete</param>
		/// <param name="transaction">Database transaction</param>
		/// <returns></returns>
		public static int DeleteByIds<T>(this IDbConnection db, IEnumerable ids, IDbTransaction transaction = null)
		{
			var type = typeof(T);
			var columns = db.GetColumns(type);
			var primary = columns.SingleOrDefault(x => x.IsPrimary);
			if (primary == null)
				throw new Exception("Unable to find Id field for type: " + type);
			var sql = "delete from [{0}] where [{1}] in @Ids".Fmt(db.GetTableName(type), primary.Name);
			return db.Execute(sql, new
				{
					Ids = ids.Cast<object>()
				}, transaction);
		}
		/// <summary>
		/// Gets the item represented by the item representing the where condition
		/// </summary>
		/// <typeparam name="T">Type of object to retrieve</typeparam>
		/// <param name="db">Database connection</param>
		/// <param name="where">Anonymous object representing where clause</param>
		/// <param name="transaction">Database transaction</param>
		/// <returns></returns>
		public static T SingleOrDefault<T>(this IDbConnection db, object where, IDbTransaction transaction = null)
		{
			if (where is string)
				throw new Exception("Please use db.Query<T>() instead");
			var sql = "select top 1 * from [{0}]".Fmt(db.GetTableName<T>());
			sql += BuildWhereClauseFromAnonymousObject(where);
			return db.Query<T>(sql, where, transaction).SingleOrDefault();
		}
		/// <summary>
		/// Get a list of objects based on the anonymous object representing the sql where clause
		/// </summary>
		/// <typeparam name="T">Type of object to query</typeparam>
		/// <param name="db">Database connection</param>
		/// <param name="where">Anonymous object representing where clause</param>
		/// <param name="transaction">Db transaction</param>
		/// <returns></returns>
		public static IList<T> Query<T>(this IDbConnection db, object where, IDbTransaction transaction = null)
		{
			if (where is string)
				throw new Exception("Please use db.Query<T>() instead");
			var sql = "select * from [{0}]".Fmt(db.GetTableName<T>());
			sql += BuildWhereClauseFromAnonymousObject(where);
			return db.Query<T>(sql, where, transaction).ToList();
		}
		public static T GetByIdOrDefault<T>(this IDbConnection db, object id, IDbTransaction transaction = null)
		{
			return db.SingleOrDefault<T>(new { id }, transaction);
		}
		/// <summary>
		/// Gets paged list of items from sql
		/// </summary>
		/// <typeparam name="T">Type of object to retrieve</typeparam>
		/// <param name="db">Database connection</param>
		/// <param name="page">Page to retrieve - starts at 1</param>
		/// <param name="pageSize">Number of items to include per page</param>
		/// <param name="where">string representing where clause</param>
		/// <param name="orderBy">Order by column(s)</param>
		/// <returns></returns>
		public static string PagedListSql<T>(this IDbConnection db, int page, int pageSize, string where = null, string orderBy = null)
		{
			var tableName = db.GetTableName<T>();

			var startRow = ((page - 1) * pageSize) + 1;
			var maxRow = (long) startRow + pageSize;
			var sql = new StringBuilder();
			sql.Append(@"select * from (select *, ROW_NUMBER() OVER (order by ");
			if (string.IsNullOrEmpty(orderBy))
			{
				var columns = db.GetColumns<T>();
				var primary = columns.FirstOrDefault(x => x.IsPrimary);
				if (primary == null)
					throw new Exception("Unable to find Id field for type: " + typeof(T));
				sql.Append("[");
				sql.Append(primary.Name);
				sql.Append("]");
			}
			else
			{
				sql.Append(orderBy);
			}
			sql.Append(") AS RowNumber FROM [{0}]".Fmt(tableName));
			if (where != null)
			{
				sql.Append(" where ");
				sql.Append(where);
			}
			sql.AppendFormat(") q where q.RowNumber between {0} and {1} order by q.RowNumber", startRow, maxRow);
			return sql.ToString();
		}

		/// <summary>
		/// Execute a stored procedure with the passed in parameters
		/// </summary>
		/// <typeparam name="T">Stored Procedure to execute</typeparam>
		/// <param name="db">Database connection</param>
		/// <param name="param">Anonymous object representing parameters to pass into procedure</param>
		public static void ExecuteProcedure<T>(this IDbConnection db, object param) where T : StoredProcedure, new()
		{
			var paramNames = GetPropertyNamesByType(param.GetType());
			var sql = "exec {0}".Fmt(new T().Name);
			var isFirst = true;
			foreach (var paramName in paramNames)
			{
				if (!isFirst)
					sql += ",";
				sql += " @" + paramName;
				isFirst = false;
			}
			db.Execute(sql, param);
		}

		/// <summary>
		/// Efficiently insert multiple items into the database. Will insert in chunks of 5,000
		/// </summary>
		/// <typeparam name="T">Type of object to insert</typeparam>
		/// <param name="db">Database connection</param>
		/// <param name="items">Items to insert</param>
		/// <param name="transaction">Transaction to use while inserting items</param>
		/// <returns></returns>
		public static int BulkInsert<T>(this IDbConnection db, IEnumerable<T> items, SqlTransaction transaction = null) where T : class
		{
			var totalCount = 0;
			var connection = db as SqlConnection;
			var profiledDbConnection = db as ProfiledDbConnection;
			if (profiledDbConnection != null)
			{
				connection = profiledDbConnection.InnerConnection as SqlConnection;
			}
			if (connection == null)
				throw new Exception("Unable to find SqlConnection from IDbConnection");
			using (var bulkInsert = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
			{
				bulkInsert.DestinationTableName = db.GetTableName<T>();
				var count = 0;
				DataTable dt = null;
				var columns = db.GetColumns<T>();
				foreach (var item in items)
				{
					if (count == 0)
					{
						dt = new DataTable(bulkInsert.DestinationTableName);
						foreach (var column in columns)
						{
							dt.Columns.Add(column.Name, column.Type);
							bulkInsert.ColumnMappings.Add(column.Name, column.Name);
						}
					}

					UpdateAuditFields(item);

					var row = dt.NewRow();
					foreach (var column in columns)
					{
						row.SetField(column.Name, column.GetValue(item));
					}
					dt.Rows.Add(row);

					totalCount++;
					// Insert in blocks of 5,000 rows
					if (++count % 5000 == 0)
					{
						bulkInsert.WriteToServer(dt);
						dt.Clear();
					}

				}
				if (dt != null && dt.Rows.Count > 0)
				{
					bulkInsert.WriteToServer(dt);
				}
			}
			return totalCount;
		}

		private static string BuildWhereClauseFromAnonymousObject(object where)
		{
			var properties = GetPropertyNamesByType(where.GetType());

			var sql = new StringBuilder();
			var isFirst = true;
			foreach (var property in properties)
			{
				if (!isFirst)
					sql.Append(" and ");
				sql.Append("[");
				sql.Append(property);
				sql.Append("]=");
				sql.Append("@");
				sql.Append(property);
				isFirst = false;
			}
			if (!isFirst)
			{
				sql.Insert(0, " where ");
			}
			return sql.ToString();
		}
		private static readonly ConcurrentDictionary<RuntimeTypeHandle, IList<string>> PropertyNamesByType = new ConcurrentDictionary<RuntimeTypeHandle, IList<string>>();
		private static IList<String> GetPropertyNamesByType(Type type)
		{
			IList<string> names;
			if (!PropertyNamesByType.TryGetValue(type.TypeHandle, out names))
			{
				names = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(prop => prop.Name).ToList();
				PropertyNamesByType.TryAdd(type.TypeHandle, names);
			}
			return names;
		}
		private static object GetDefault(Type type)
		{
			if (type.IsValueType)
			{
				return Activator.CreateInstance(type);
			}
			return null;
		}
		private static void UpdateAuditFields(object obj)
		{
			var entityType = obj.GetType();
			int? userId = null;
			UserPrincipal currentUser;
			if (HttpContext.Current != null)
			{
				currentUser = HttpContext.Current.User as UserPrincipal;
			}
			else
			{
				currentUser = Thread.CurrentPrincipal as UserPrincipal;
			}
			if (currentUser != null && currentUser.UserObject != null && currentUser.UserObject.Id != default(int))
				userId = currentUser.UserObject.Id;

			if (userId.HasValue)
			{
				var createdBy = entityType.GetProperty("CreatedBy");
				if (createdBy != null && (int) createdBy.GetValue(obj) == default(int))
					createdBy.SetValue(obj, userId.Value);

				var modifiedBy = entityType.GetProperty("ModifiedBy");
				if (modifiedBy != null)
					modifiedBy.SetValue(obj, userId.Value);
			}

			var createdOn = entityType.GetProperty("CreatedOn");
			if (createdOn != null && (createdOn.GetValue(obj) == null || (DateTime) createdOn.GetValue(obj) == default(DateTime)))
				createdOn.SetValue(obj, DateTime.UtcNow);
			var modifiedOn = entityType.GetProperty("ModifiedOn");
			if (modifiedOn != null)
				modifiedOn.SetValue(obj, DateTime.UtcNow);
		}

	}
}