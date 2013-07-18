using System;
using System.Data;
using KickstartTemplate.Infrastructure.Extensions;
using KickstartTemplate.Models.Users;
using Spruce;
using Spruce.Migrations;

namespace KickstartTemplate.Infrastructure.Data.Migrations
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