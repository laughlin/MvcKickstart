using System.Data;
using CacheStack;
using MvcKickstart.Infrastructure;
using MvcKickstart.Models.Users;
using ServiceStack.CacheAccess;
using Spruce;

namespace MvcKickstart.Services
{
	public interface IUserService
	{
		User GetById(int id);
		User GetByUsername(string username);
	}

	public class UserService : IUserService
	{
		protected IDbConnection Db { get; set; }
		protected ICacheClient Cache { get; set; }
		public UserService(IDbConnection db, ICacheClient cache)
		{
			Db = db;
			Cache = cache;
		}

		public User GetById(int id)
		{
			return Cache.GetOrCache(CacheKeys.User.ById(id), context =>
				{
					var item = Db.GetByIdOrDefault<User>(id);
					if (item != null)
						context.InvalidateOn(TriggerFrom.Id<User>(item.Id));
					return item;
				});
		}
		public User GetByUsername(string username)
		{
			return Cache.GetOrCache(CacheKeys.User.ByUsername(username), context =>
				{
					var item = Db.SingleOrDefault<User>(new { username });
					if (item != null)
						context.InvalidateOn(TriggerFrom.Id<User>(item.Id));
					return item;
				});
		}
	}
}