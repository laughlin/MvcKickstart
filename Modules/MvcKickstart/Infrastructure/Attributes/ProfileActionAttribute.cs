using System.Diagnostics;
using System.Web.Mvc;
using StructureMap;

namespace MvcKickstart.Infrastructure.Attributes
{
	public class ProfileActionAttribute : ActionFilterAttribute
	{
		private IMetricTracker Metrics { get; set; }

		public ProfileActionAttribute()
		{
			Metrics = ObjectFactory.GetInstance<IMetricTracker>();
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (!filterContext.IsChildAction)
			{
				var stopwatch = new Stopwatch();
				filterContext.HttpContext.Items[ViewDataConstants.ProfileActionStopwatch] = stopwatch;

				stopwatch.Start();
			}
			base.OnActionExecuting(filterContext);
		}

		public override void OnResultExecuted(ResultExecutedContext filterContext)
		{
			base.OnResultExecuted(filterContext);
			if (filterContext.IsChildAction)
				return;

			var stopwatch = (Stopwatch) filterContext.HttpContext.Items[ViewDataConstants.ProfileActionStopwatch];
			if (stopwatch == null)
				return;
			stopwatch.Stop();
			Metrics.Timing(MetricBase.Profiling_RenderTime, stopwatch.Elapsed.TotalMilliseconds);
			filterContext.HttpContext.Items[ViewDataConstants.ProfileActionStopwatch] = null;
		}
	}
}