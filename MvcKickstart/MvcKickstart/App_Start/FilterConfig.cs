using System.Web;
using System.Web.Mvc;
using MvcKickstart.Infrastructure.Attributes;

namespace MvcKickstart
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandlesErrorAttribute());
			filters.Add(new ProfileActionAttribute());
			filters.Add(new StackExchange.Profiling.MVCHelpers.ProfilingActionFilter());
			filters.Add(new TrackAuthenticationMetricsAttribute());
		}
	}
}