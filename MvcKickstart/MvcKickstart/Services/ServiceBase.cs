using System;
using System.Data;
using CacheStack;
using Dapper;
using ServiceStack.CacheAccess;
using ServiceStack.Text;
using Spruce;

namespace MvcKickstart.Services
{
	public interface IServiceBase<T> where T : class
	{
		T Save(T item);
		T GetByIdOrDefault(object id);
		void SoftDelete(T item);
		void SoftUndelete(T item);
	}

	public abstract class ServiceBase<T> : IServiceBase<T> where T : class
	{
		protected IDbConnection Db { get; set; }
		protected ICacheClient Cache { get; set; }

		protected ServiceBase(IDbConnection db, ICacheClient cache)
		{
			Db = db;
			Cache = cache;
		}

		protected abstract string GetIdCacheKey(object id);
		protected object GetId(T item)
		{
			var type = item.GetType();
			var idProperty = type.GetProperty("Id");
			if (idProperty == null)
				throw new Exception("Object does not have Id property. Type: " + type);

			return idProperty.GetValue(item);
		}

		public virtual T Save(T item)
		{
			var savedItem = Db.Save(item);

			var id = GetId(item);

			Cache.Trigger(TriggerFor.Id<T>(id));

			return savedItem;
		}

		public virtual T GetByIdOrDefault(object id)
		{
			return Cache.GetOrCache(GetIdCacheKey(id), context =>
				{
					var item = Db.GetByIdOrDefault<T>(id);
					if (item != null)
						context.InvalidateOn(TriggerFrom.Id<T>(id));
					return item;
				});
		}

		public virtual void SoftDelete(T item)
		{
			var id = GetId(item);

			Db.Execute("update [{0}] set IsDeleted=1 where Id=@Id".Fmt(
				Db.GetTableName<T>()
			), new
				{
					id
				});

			Cache.Trigger(TriggerFor.Id<T>(id));
		}

		public virtual void SoftUndelete(T item)
		{
			var id = GetId(item);

			Db.Execute("update [{0}] set IsDeleted=0 where Id=@Id".Fmt(
				Db.GetTableName<T>()
			), new
				{
					id
				});

			Cache.Trigger(TriggerFor.Id<T>(id));
		}
	}
}