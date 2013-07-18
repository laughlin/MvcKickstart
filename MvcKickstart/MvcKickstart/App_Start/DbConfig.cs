using System;
using System.Data.SqlClient;
using System.Threading;
using System.Web;
using KickstartTemplate.Infrastructure;
using KickstartTemplate.Infrastructure.Data;
using Spruce;
using StructureMap;

namespace KickstartTemplate
{
	public class DbConfig
	{
		public static void Setup()
		{
			// Update audit fields before an item is saved
			SpruceSettings.Saving += (item, e) =>
				{
					var entityType = item.GetType();
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
						if (createdBy != null && (int) createdBy.GetValue(item) == default(int))
							createdBy.SetValue(item, userId.Value);

						var modifiedBy = entityType.GetProperty("ModifiedBy");
						if (modifiedBy != null)
							modifiedBy.SetValue(item, userId.Value);
					}

					var createdOn = entityType.GetProperty("CreatedOn");
					if (createdOn != null && (createdOn.GetValue(item) == null || (DateTime) createdOn.GetValue(item) == default(DateTime)))
						createdOn.SetValue(item, DateTime.UtcNow);
					var modifiedOn = entityType.GetProperty("ModifiedOn");
					if (modifiedOn != null)
						modifiedOn.SetValue(item, DateTime.UtcNow);
				};
		}
		public static void Initialize()
		{
			Setup();

			using (var db = ObjectFactory.GetInstance<SqlConnection>())
			{
				db.Open();			
				DataMigrator.RunMigrations(db);
			}
		}
	}
}