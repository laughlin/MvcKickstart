using System;
using MvcKickstart.Models;

namespace MvcKickstart.Infrastructure.Data.Schema.Sections
{
	/// <summary>
	/// These are db objects that don't really fit anywhere else. They are core to the system as a whole
	/// </summary>
	public class GlobalSection : ISchemaSection
	{
		public Type[] Tables
		{
			get
			{
				return new []
				{
					typeof(DataMigration),
				};
			}
		}

		public ScriptedObject[] ScriptedObjects
		{
			get
			{
				return new ScriptedObject[]
				{
				};
			}
		}
	}
}