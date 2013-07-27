using System.Web.Mvc;
using MvcKickstart.Infrastructure.Attributes;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof($rootnamespace$.FiltersConfig), "PreStart", Order = 100)]

namespace $rootnamespace$ 
{
	public static class FiltersConfig
	{
		private static void SetupFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandlesErrorAttribute());
			filters.Add(new ProfileActionAttribute());
			filters.Add(new StackExchange.Profiling.MVCHelpers.ProfilingActionFilter());
			filters.Add(new TrackAuthenticationMetricsAttribute());			
		}

		public static void PreStart() 
		{
			SetupFilters(GlobalFilters.Filters);
		}
	}
}
