using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcKickstart.Infrastructure.Data.Schema
{
	public class Column
	{
		public string Name { get; set; }
		public bool IsPrimary { get; set; }
		public bool IsNullable { get; set; }
		public Type Type { get; set; }
		public string SqlType { get; set; }
		public bool AutoIncrement { get; set; }
		public string DefaultValue { get; set; }
		public bool HasForeignKey { get; set; }
		public string ForeignKeyName { get; set; }
		public string ReferencedTableName { get; set; }
		public Func<object, object> GetValue { get; set; }
	}
}