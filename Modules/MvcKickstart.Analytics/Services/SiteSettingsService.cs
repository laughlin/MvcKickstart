using System.Data;
using System.Linq;
using CacheStack;
using Dapper;
using MvcKickstart.Analytics.Infrastructure;
using MvcKickstart.Analytics.Models;
using ServiceStack.CacheAccess;
using ServiceStack.Text;
using Spruce;

namespace MvcKickstart.Analytics.Services
{
	public interface ISiteSettingsService
	{
		SiteSettings GetSettings();
	}

	public class SiteSettingsService : ISiteSettingsService
	{
		protected IDbConnection Db { get; set; }
		protected ICacheClient Cache { get; set; }
		public SiteSettingsService(IDbConnection db, ICacheClient cache)
		{
			Db = db;
			Cache = cache;
		}

		public SiteSettings GetSettings()
		{
			return Cache.GetOrCache(CacheKeys.SiteSettings, context =>
				{
					var item = Db.Query<SiteSettings>("select top 1 * from [{0}]".Fmt(Db.GetTableName<SiteSettings>())).SingleOrDefault();
					if (item != null)
						context.InvalidateOn(TriggerFrom.Any<SiteSettings>());
					return item;
				}) ?? new SiteSettings();
		}
	}
}