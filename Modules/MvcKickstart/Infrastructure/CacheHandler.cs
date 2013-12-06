using System;
using System.Web;
using ServiceStack.CacheAccess;
using StructureMap;

namespace MvcKickstart.Infrastructure
{
	public class CacheHandler : IHttpHandler
	{
		public bool IsReusable
		{
			get { return false; }
		}

		public void ProcessRequest(HttpContext context)
		{
			Action<ICacheClientBroadcaster> action = null;
			switch ((context.Request.QueryString["action"] ?? string.Empty).Trim().ToLower())
			{
				case "remove":
					action = cache => cache.Remove(context.Request.QueryString["key"], false);
					break;
				case "flushall":
					action = cache => cache.FlushAll(false);
					break;
			}

			if (action == null)
				return;

			var cacheClient = ObjectFactory.GetInstance<ICacheClient>() as ICacheClientBroadcaster;
			if (cacheClient != null)
				action(cacheClient);
		}
	}
}
