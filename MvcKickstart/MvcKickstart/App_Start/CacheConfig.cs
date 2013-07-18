using System.Collections.Generic;
using CacheStack;
using KickstartTemplate.Infrastructure;
using KickstartTemplate.Models.Users;
using ServiceStack.CacheAccess;
using StructureMap;

namespace KickstartTemplate
{
	public class CacheConfig
	{
		public static void Initialize()
		{
			CacheStackSettings.CacheClient = ObjectFactory.GetInstance<ICacheClient>();
			// All of our routes are unique and not shared, so we can use the route name instead of reflection to get a unique cache key
			CacheStackSettings.UseRouteNameForCacheKey = true;

			// Share same objects between different cache keys
			CacheStackSettings.CacheKeysForObject.Add(typeof(User), item => {
				var userItem = item as User;
				var keys = new List<string>
					{
						CacheKeys.Users.ById(userItem.Id), 
						CacheKeys.Users.ByUsername(userItem.Username)
					};
				return keys;
			});
		}
	}
}