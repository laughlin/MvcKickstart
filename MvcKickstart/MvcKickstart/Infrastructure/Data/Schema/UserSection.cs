using System;
using KickstartTemplate.Models.Users;
using Spruce.Schema;

namespace KickstartTemplate.Infrastructure.Data.Schema
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