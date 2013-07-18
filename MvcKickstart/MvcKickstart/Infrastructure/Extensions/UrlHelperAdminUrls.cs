using System.Web.Mvc;
using System.Web.Routing;

namespace MvcKickstart.Infrastructure.Extensions
{
	public class UrlHelperAdminUrls
	{
		public UrlHelper Url { get; private set; }

		public UrlHelperAdminUrls(UrlHelper helper)
		{
			Url = helper;
		}

		#region Home

		public HomeUrls Home()
		{
			return new HomeUrls(Url);
		}
		public class HomeUrls
		{
			public UrlHelper Url { get; set; }
			public HomeUrls(UrlHelper url)
			{
				Url = url;
			}

			public string Index()
			{
				return Url.RouteUrl("Admin_Home_Index");
			}

			public string AuthResponse()
			{
				return Url.RouteUrl("Admin_Home_AuthResponse");
			}
		}

		#endregion

		public abstract class RestUrls<T>
		{
			public abstract string UrlPrefix { get; }
			protected UrlHelper Url { get; set; }
			public string IdName { get; set; }

			protected RestUrls(UrlHelper url)
			{
				Url = url;
				IdName = "id";
			}

			public string Index()
			{
				return Url.RouteUrl(UrlPrefix + "Index");
			}
			public string New()
			{
				return Url.RouteUrl(UrlPrefix + "New");
			}
			public string Details(T id)
			{
				return Url.RouteUrl(UrlPrefix + "Details", new RouteValueDictionary {{ IdName, id }});
			}
			public string Edit(T id)
			{
				return Url.RouteUrl(UrlPrefix + "Edit", new RouteValueDictionary { { IdName, id } });
			}
			public string Delete(T id)
			{
				return Url.RouteUrl(UrlPrefix + "Delete", new RouteValueDictionary { { IdName, id } });
			}
			public string UnDelete(T id)
			{
				return Url.RouteUrl(UrlPrefix + "UnDelete", new RouteValueDictionary { { IdName, id } });
			}
			public string Grid()
			{
				return Url.RouteUrl(UrlPrefix + "Grid");
			}
		}

	}
}