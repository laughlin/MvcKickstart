using System;
using MvcKickstart.Models.Users;

namespace MvcKickstart.Infrastructure.Data.Schema.Sections
{
	public class UserSection : ISchemaSection
	{
		public Type[] Tables
		{
			get
			{
				return new []
				{
					typeof(User),
					typeof(PasswordRetrieval),
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