using System.Data;
using CacheStack;
using MvcKickstart.Infrastructure;
using MvcKickstart.Models.Users;
using ServiceStack.CacheAccess;
using Spruce;

namespace MvcKickstart.Services
{
	public interface IUserService : IServiceBase<User>
	{
		User GetByUsername(string username);
	}

	public class UserService : ServiceBase<User>, IUserService
	{
		public UserService(IDbConnection db, ICacheClient cache) : base(db, cache)
		{
		}

		protected override string GetIdCacheKey(object id)
		{
			return CacheKeys.Users.ById((int)id);
		}

		public User GetByUsername(string username)
		{
			return Cache.GetOrCache(CacheKeys.Users.ByUsername(username), context =>
				{
					var item = Db.SingleOrDefault<User>(new { username });
					if (item != null)
						context.InvalidateOn(TriggerFrom.Id<User>(item.Id));
					return item;
				});
		}
	}
}