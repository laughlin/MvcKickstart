using System.Web.Mvc;

namespace MvcKickstart.Analytics.Infrastructure.Extensions
{
	public static class UrlHelperExtensions
	{
		public static string AdminHome(this UrlHelper url)
		{
			return url.RouteUrl("Admin_Home_Index");
		}
		public static AnalyticsWidgetUrls AnalyticsWidget(this UrlHelper url)
		{
			return new AnalyticsWidgetUrls(url);
		}

		public class AnalyticsWidgetUrls
		{
			protected UrlHelper Url { get; private set; }
			public AnalyticsWidgetUrls(UrlHelper url)
			{
				Url = url;
			}

			public string AuthResponse()
			{
				return Url.RouteUrl("MvcKickstart_Analytics_Widgets_AuthResponse");
			}
			public string Analytics()
			{
				return Url.RouteUrl("MvcKickstart_Analytics_Widgets_Analytics");
			}
		}
	}
}
