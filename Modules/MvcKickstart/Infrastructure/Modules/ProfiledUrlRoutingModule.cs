using System;
using System.Web;
using System.Web.Routing;
using StackExchange.Profiling;

namespace MvcKickstart.Infrastructure.Modules
{
	public class ProfiledUrlRoutingModule : UrlRoutingModule
	{
		public override void PostResolveRequestCache(HttpContextBase context)
		{
			// Avoid routing hit for static file directories
			var isStatic = context.Request.Path.StartsWith("/Content/images/", StringComparison.OrdinalIgnoreCase) ||
						   context.Request.Path.StartsWith("/favicon", StringComparison.OrdinalIgnoreCase) ||
						   context.Request.Path.StartsWith("/cassette.axd", StringComparison.OrdinalIgnoreCase);
			if (isStatic)
			{
				MiniProfiler.Stop(true);
				return;
			}

			using (new MetricStopwatch(MetricBase.Profiling_ResolveRoute))
			{
				base.PostResolveRequestCache(context);
			}
		}
	}
}