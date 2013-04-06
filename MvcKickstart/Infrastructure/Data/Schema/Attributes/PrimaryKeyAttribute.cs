using System;

namespace MvcKickstart.Infrastructure.Data.Schema.Attributes
{
	/// <summary>
	/// Indicates that this property should be the primary key of the table
	/// </summary>
	[AttributeUsage( AttributeTargets.Property)]
	public class PrimaryKeyAttribute : Attribute
	{
	}
}