using System;
using System.Data;
using MvcKickstart.Infrastructure.Extensions;
using MvcKickstart.Models.Users;
using Spruce;
using Spruce.Migrations;

namespace MvcKickstart.Infrastructure.Data.Migrations
{
	public class InitializeDbMigration : IMigration
	{
		public int Order { get { return 20130330; } }

		public void Execute(IDbConnection db)
		{
			db.Save(new User
					{
					    Username = "admin",
					    Password = "admin".ToSHAHash(),
						IsAdmin = true,
					    Email = "notset@localhost.com",
					});
		}
	}
}