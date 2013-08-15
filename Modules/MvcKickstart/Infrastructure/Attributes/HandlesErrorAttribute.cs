using System.Web.Mvc;
using ServiceStack.Logging;
using StructureMap;

namespace MvcKickstart.Infrastructure.Attributes
{
	public class HandlesErrorAttribute : HandleErrorAttribute
	{
		private IMetricTracker Metrics { get; set; }
		private ILog Log { get; set; }

		public HandlesErrorAttribute()
		{
			Metrics = ObjectFactory.GetInstance<IMetricTracker>();
			Log = LogManager.LogFactory.GetLogger(GetType());
		}

		public override void OnException(ExceptionContext filterContext)
		{
			Metrics.Increment(MetricBase.Error_Unhandled);
			if (filterContext != null && filterContext.Exception != null)
			{
				Log.Error("Unhandled exception", filterContext.Exception);
			}
			base.OnException(filterContext);
		}
	}
}