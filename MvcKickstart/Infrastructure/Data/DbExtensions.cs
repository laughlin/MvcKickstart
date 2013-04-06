using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Dapper;
using MvcKickstart.Infrastructure.Data.Schema;
using MvcKickstart.Infrastructure.Data.Schema.Attributes;
using MvcKickstart.Infrastructure.Extensions;
using ServiceStack.Common.Extensions;
using ServiceStack.Text;

namespace MvcKickstart.Infrastructure.Data
{
	/// <summary>
	/// These extensions are only really useful in the data namespace, so they are separate from the other DbConnectionExtensions.
	/// </summary>
	/// <remarks></remarks>
	public static class DbExtensions
	{
		public static bool TableExists(this IDbConnection db, string name, IDbTransaction transaction = null)
        {
            string schemaName = null;

            name = name.Replace("[", "");
            name = name.Replace("]", "");

            if(name.Contains("."))
            {
                var parts = name.Split('.');
                if (parts.Count() == 2)
                {
                    schemaName = parts[0];
                    name = parts[1];
                }
            }

            var builder = new StringBuilder("select 1 from INFORMATION_SCHEMA.TABLES where ");
            if (!String.IsNullOrEmpty(schemaName)) 
				builder.Append("TABLE_SCHEMA = @schemaName AND ");
            builder.Append("TABLE_NAME = @name");

            return db.Query(builder.ToString(), new { schemaName, name }, transaction: transaction).Count() == 1;
        }

				/// <summary>
		/// Creates the sql table for the specified type
		/// </summary>
		/// <param name="db"></param>
		/// <typeparam name="T">Type representing the table to create</typeparam>
		public static void CreateTable<T>(this IDbConnection db)
		{
			db.CreateTable(typeof(T));
		}

		/// <summary>
		/// Creates the sql table for the specified type
		/// </summary>
		/// <param name="db"></param>
		/// <param name="type">Type representing the table to create</param>
		public static void CreateTable(this IDbConnection db, Type type)
		{
			var tableName = db.GetTableName(type);

			var text = new StringBuilder();
			var constraints = new StringBuilder();
			text.Append("CREATE TABLE [");
			text.Append(tableName);
			text.AppendLine("] (");

			var columns = db.GetColumns(type);
			var columnIndex = 0;
			foreach (var column in columns)
			{
				var isFirst = columnIndex++ == 0;
				if (!isFirst)
				{
					text.AppendLine(",");
				}
				
				text.AppendFormat("\"{0}\" {1}", column.Name, column.SqlType);
				if (column.IsPrimary)
				{
					text.Append(" PRIMARY KEY");
					if (column.AutoIncrement)
					{
						text.Append(" IDENTITY");
					}
				}
				else
				{
					if (column.IsNullable)
					{
						text.Append(" NULL");
					}
					else
					{
						text.Append(" NOT NULL");
					}
				}
				if (!string.IsNullOrEmpty(column.DefaultValue))
				{
					text.AppendFormat(" DEFAULT ({0})", column.DefaultValue);
				}

				if (column.HasForeignKey)
				{
					constraints.Append(", CONSTRAINT \"");
					constraints.Append(column.ForeignKeyName);
					constraints.Append("\" FOREIGN KEY (\"");
					constraints.Append(column.Name);
					constraints.Append("\") REFERENCES \"");
					constraints.Append(column.ReferencedTableName);
					constraints.AppendLine("\" (\"Id\")");
				}
			}
			if (constraints.Length > 0)
				text.Append(constraints);

			text.AppendLine(");");

			db.Execute(text.ToString());
		}
		private static readonly ConcurrentDictionary<RuntimeTypeHandle, IList<Column>> Columns = new ConcurrentDictionary<RuntimeTypeHandle, IList<Column>>();
		public static IList<Column> GetColumns<T>(this IDbConnection db)
		{
			return db.GetColumns(typeof(T));
		}
		public static IList<Column> GetColumns(this IDbConnection db, Type type)
		{
			IList<Column> columns;
			if (!Columns.TryGetValue(type.TypeHandle, out columns))
			{
				columns = new List<Column>();
				var columnIndex = 0;
				var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
				var hasIdField = properties.Any(x => x.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
				string tableName = null;
				foreach (var property in properties)
				{
					var ignore = property.FirstAttribute<IgnoreAttribute>();
					if (ignore != null)
						continue;

					// Only map properties with get and set accessors
					if (!property.CanWrite || !property.CanRead)
						continue;

					var isFirst = columnIndex++ == 0;
					var pkAttribute = property.FirstAttribute<PrimaryKeyAttribute>();
					var isPrimary = (!hasIdField && isFirst) || pkAttribute != null || property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase);
					var isNullableType = property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
					var isNullable = (!property.PropertyType.IsValueType && property.FirstAttribute<RequiredAttribute>() == null) || isNullableType;
					var propertyType = isNullableType
						? Nullable.GetUnderlyingType(property.PropertyType)
						: property.PropertyType;
					var sqlType = GetSqlType(propertyType);
					var autoIncrement = isPrimary && property.FirstAttribute<AutoIncrementAttribute>() != null;
					var defaultValueAttribute = property.FirstAttribute<DefaultAttribute>();
					var defaultValue = defaultValueAttribute != null ? defaultValueAttribute.DefaultValue : null;

					var closureProperty = property;
					var column = new Column
						{
							IsPrimary = isPrimary,
							IsNullable = isNullable,
							Type = propertyType,
							Name = property.Name,
							SqlType = sqlType,
							AutoIncrement = autoIncrement,
							DefaultValue = defaultValue,
							GetValue = closureProperty.GetValue
						};

					var referencesAttribute = property.FirstAttribute<ReferencesAttribute>();
					if (referencesAttribute != null)
					{
						if (tableName == null)
							tableName = db.GetTableName(type);

						var referencedClassType = referencesAttribute.Type;
						column.HasForeignKey = true;
						column.ReferencedTableName = db.GetTableName(referencedClassType);
						column.ForeignKeyName = GetForeignKeyName(tableName, property.Name, column.ReferencedTableName);
					}

					columns.Add(column);
				}
				Columns.TryAdd(type.TypeHandle, columns);
			}
			return columns;
		}


		private static string GetForeignKeyName(string tableName, string columnName, string referencedTableName)
		{
			return "FK_{0}_{1}_{2}".Fmt(tableName, referencedTableName, columnName);
		}

		private static readonly IDictionary<Type, string> TypeToSqlType = new Dictionary<Type, string>
			{
				{ typeof(string), "nvarchar({0})" },
				{ typeof(bool), "bit" },
				{ typeof(int), "int" },
				{ typeof(long), "bigint" },
				{ typeof(double), "double" },
				{ typeof(decimal), "decimal(18,2)" },
				{ typeof(Guid), "uniqueidentifier" },
				{ typeof(DateTime), "DATETIME2(7)" },
				{ typeof(TimeSpan), "bigint" },
				{ typeof(Enum), "int" },
			};
		private static string GetSqlType(Type type)
		{
			string sqlType;
			var isTypeDefined = TypeToSqlType.TryGetValue(type, out sqlType);

			// Default to nvarchar(50) if the field type is not defined.
			if (!isTypeDefined)
				sqlType = TypeToSqlType[typeof(string)].Fmt(50);

			if (type == typeof(string))
			{
				var stringLength = "MAX";
				var stringLengthAttribute = type.FirstAttribute<StringLengthAttribute>(true);
				if (stringLengthAttribute != null)
				{
					stringLength = stringLengthAttribute.MaximumLength.ToString();
				}
				sqlType = sqlType.Fmt(stringLength);
			}

			return sqlType;
		}
	}
}