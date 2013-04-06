using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Design.PluralizationServices;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using Dapper;
using MvcKickstart.Infrastructure.Data;
using MvcKickstart.Infrastructure.Data.Schema;

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
					var scriptedObject = (ScriptedObject)Activator.CreateInstance(type);
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
		/// <returns></returns>
		public static T Save<T>(this IDbConnection db, T item) where T : class
		{
			UpdateAuditFields(item);

			var type = item.GetType();
			var prop = type.GetProperty("Id");
			var isInsert = Equals(prop.GetValue(item), GetDefault(prop.PropertyType));
			if (isInsert)
			{
				var id = db.Insert(item);
				if (prop.PropertyType == typeof(int))
				{
					prop.SetValue(item, (int)id, null);
				}
				else
				{
					prop.SetValue(item, id, null);
				}
			}
			else
			{
				db.Update(item);
			}
			return item;
		}
		private static long Insert<T>(this IDbConnection db, T item)
		{
			var sql = new StringBuilder();
			sql.AppendFormat("insert into {0} (", db.GetTableName<T>());
			var columns = db.GetColumns<T>().Where(x => !x.IsPrimary).ToList();
			
			for (var i = 0; i < columns.Count; i++)
			{
				var column = columns.ElementAt(i);
				sql.Append(column.Name);
				if (i != columns.Count -1)
				{
					sql.Append(",");
				}
			}
			sql.Append(") values (");
			for (var i = 0; i < columns.Count; i++)
			{
				var column = columns.ElementAt(i);
				sql.AppendFormat("@{0}", column.Name);
				if (i != columns.Count -1)
				{
					sql.Append(",");
				}
			}
			sql.AppendLine(")");
			sql.AppendLine("select cast(SCOPE_IDENTITY() as bigint)");
			return db.Query<long>(sql.ToString(), item).Single();
		}
		private static void Update<T>(this IDbConnection db, T item)
		{
			var sql = new StringBuilder();
			sql.AppendFormat("update [{0}] set ", db.GetTableName<T>());
			var columns = db.GetColumns<T>();
			var columnsWithoutPrimary = columns.Where(x => !x.IsPrimary).ToList();
			for (var i = 0; i < columnsWithoutPrimary.Count; i++)
			{
				var column = columnsWithoutPrimary.ElementAt(i);
				sql.AppendFormat("{0}=@{0}", column.Name);
				if (i != columnsWithoutPrimary.Count -1)
				{
					sql.Append(",");
				}
			}
			sql.AppendFormat(" where {0}=@{0}", columns.First(x => x.IsPrimary).Name);
			db.Execute(sql.ToString(), item);
		}


		public static int BulkInsert<T>(this IDbConnection db, IEnumerable<T> items, SqlTransaction transaction = null) where T: class
		{
			var totalCount = 0;
			using (var bulkInsert = new SqlBulkCopy((SqlConnection)db, SqlBulkCopyOptions.Default, transaction))
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
					if (++count%5000 == 0)
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
			if (createdOn != null && (DateTime) createdOn.GetValue(obj) == default(DateTime))
				createdOn.SetValue(obj, DateTime.UtcNow);
			var modifiedOn = entityType.GetProperty("ModifiedOn");
			if (modifiedOn != null)
				modifiedOn.SetValue(obj, DateTime.UtcNow);
		}

	}
}