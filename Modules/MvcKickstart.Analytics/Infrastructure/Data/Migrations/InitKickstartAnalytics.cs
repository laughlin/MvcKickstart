using System;
using System.Data;
using MvcKickstart.Analytics.Models;
using Spruce;
using Spruce.Schema;
using Spruce.Migrations;

namespace MvcKickstart.Analytics.Infrastructure.Data.Migrations
{
	public class InitKickstartAnalytics : IMigration
	{
		public int Order { get { return 1000; } }

		public Type[] ScriptedObjectsToRecreate
		{
			get
			{
				return null;
			}
		}

		public void Execute(IDbConnection db)
		{
			if (!db.TableExists(db.GetTableName<SiteSettings>()))
			{
				db.CreateTable<SiteSettings>();
			}
			else
			{
				// Append the analytics specific columns
				db.AddColumn<SiteSettings>(x => x.AnalyticsProfileId);
				db.AddColumn<SiteSettings>(x => x.AnalyticsToken);
			}
		}
	}
}
