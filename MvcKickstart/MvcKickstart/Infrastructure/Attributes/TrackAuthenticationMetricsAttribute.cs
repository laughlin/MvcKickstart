using System.Web.Mvc;
using StructureMap;

namespace MvcKickstart.Infrastructure.Attributes
{
	public class TrackAuthenticationMetricsAttribute : ActionFilterAttribute
	{
		private IMetricTracker Metrics { get; set; }

		public TrackAuthenticationMetricsAttribute()
		{
			Metrics = ObjectFactory.GetInstance<IMetricTracker>();
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (!filterContext.Controller.ControllerContext.IsChildAction && !filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
			{
				Metrics.Increment(filterContext.HttpContext.User.Identity.IsAuthenticated ? Metric.Users_AuthenticatedUserRequest : Metric.Users_AnonymousUserRequest);
			}

			base.OnActionExecuting(filterContext);
		}
	}
}