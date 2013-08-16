using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;

namespace KickstartTemplate.Infrastructure.Extensions
{
	public static class HtmlHelperExtensions
	{
		public static MvcHtmlString RouteAction(this HtmlHelper htmlHelper, string routeName)
		{
			return RouteAction(htmlHelper, routeName, (RouteValueDictionary) null);
		}
		public static MvcHtmlString RouteAction(this HtmlHelper htmlHelper, string routeName, object routeValues)
		{
			return RouteAction(htmlHelper, routeName, new RouteValueDictionary(routeValues));
		}
		public static MvcHtmlString RouteAction(this HtmlHelper htmlHelper, string routeName, RouteValueDictionary routeValues)
		{
			using (var stringWriter = new StringWriter((IFormatProvider) CultureInfo.CurrentCulture))
			{
				ActionHelper(htmlHelper, routeName, routeValues, (TextWriter) stringWriter);
				return MvcHtmlString.Create(stringWriter.ToString());
			}
		}

		internal static void ActionHelper(HtmlHelper htmlHelper, string routeName, RouteValueDictionary routeValues, TextWriter textWriter)
		{
			if (htmlHelper == null)
				throw new ArgumentNullException("htmlHelper");
			if (string.IsNullOrEmpty(routeName))
				throw new ArgumentNullException("routeName");

			routeValues = MergeDictionaries(routeValues, htmlHelper.ViewContext.RouteData.Values);

			var virtualPathForArea = htmlHelper.RouteCollection.GetVirtualPathForArea(htmlHelper.ViewContext.RequestContext, routeName, routeValues);

			var routeData = CreateRouteData(virtualPathForArea.Route, routeValues, virtualPathForArea.DataTokens, htmlHelper.ViewContext);
			var httpContext = htmlHelper.ViewContext.HttpContext;
			var actionMvcHandler = new ChildActionMvcHandler(new RequestContext(httpContext, routeData));
			httpContext.Server.Execute(HttpHandlerUtil.WrapForServerExecute((IHttpHandler) actionMvcHandler), textWriter, true);
		}

		private static RouteData CreateRouteData(RouteBase route, RouteValueDictionary routeValues, RouteValueDictionary dataTokens, ViewContext parentViewContext)
		{
			RouteData routeData = new RouteData();
			foreach (KeyValuePair<string, object> keyValuePair in routeValues)
				routeData.Values.Add(keyValuePair.Key, keyValuePair.Value);
			foreach (KeyValuePair<string, object> keyValuePair in dataTokens)
				routeData.DataTokens.Add(keyValuePair.Key, keyValuePair.Value);
			routeData.Route = route;
			routeData.DataTokens["ParentActionViewContext"] = (object) parentViewContext;
			return routeData;
		}

		private static RouteValueDictionary MergeDictionaries(params RouteValueDictionary[] dictionaries)
		{
			RouteValueDictionary routeValueDictionary1 = new RouteValueDictionary();
			foreach (RouteValueDictionary routeValueDictionary2 in Enumerable.Where<RouteValueDictionary>((IEnumerable<RouteValueDictionary>) dictionaries, (Func<RouteValueDictionary, bool>) (d => d != null)))
			{
				foreach (KeyValuePair<string, object> keyValuePair in routeValueDictionary2)
				{
					if (!routeValueDictionary1.ContainsKey(keyValuePair.Key))
						routeValueDictionary1.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return routeValueDictionary1;
		}

		internal class ChildActionMvcHandler : MvcHandler
		{
			public ChildActionMvcHandler(RequestContext context)
				: base(context)
			{
			}
		}
	}

	internal static class HttpHandlerUtil
	{
		public static IHttpHandler WrapForServerExecute(IHttpHandler httpHandler)
		{
			var httpHandler1 = httpHandler as IHttpAsyncHandler;
			if (httpHandler1 == null)
				return (IHttpHandler) new HttpHandlerUtil.ServerExecuteHttpHandlerWrapper(httpHandler);
			else
				return (IHttpHandler) new HttpHandlerUtil.ServerExecuteHttpHandlerAsyncWrapper(httpHandler1);
		}

		internal class ServerExecuteHttpHandlerWrapper : Page
		{
			private readonly IHttpHandler _httpHandler;

			internal IHttpHandler InnerHandler
			{
				get
				{
					return this._httpHandler;
				}
			}

			public ServerExecuteHttpHandlerWrapper(IHttpHandler httpHandler)
			{
				this._httpHandler = httpHandler;
			}

			public override void ProcessRequest(HttpContext context)
			{
				HttpHandlerUtil.ServerExecuteHttpHandlerWrapper.Wrap((Action) (() => this._httpHandler.ProcessRequest(context)));
			}

			protected static void Wrap(Action action)
			{
				HttpHandlerUtil.ServerExecuteHttpHandlerWrapper.Wrap<object>((Func<object>) (() =>
				{
					action();
					return (object) null;
				}));
			}

			protected static TResult Wrap<TResult>(Func<TResult> func)
			{
				try
				{
					return func();
				}
				catch (HttpException ex)
				{
					if (ex.GetHttpCode() != 500)
						throw new HttpException(500, "Internal error", (Exception) ex);
					throw;
				}
			}
		}

		private sealed class ServerExecuteHttpHandlerAsyncWrapper : HttpHandlerUtil.ServerExecuteHttpHandlerWrapper, IHttpAsyncHandler, IHttpHandler
		{
			private readonly IHttpAsyncHandler _httpHandler;

			public ServerExecuteHttpHandlerAsyncWrapper(IHttpAsyncHandler httpHandler)
				: base((IHttpHandler) httpHandler)
			{
				this._httpHandler = httpHandler;
			}

			public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
			{
				return HttpHandlerUtil.ServerExecuteHttpHandlerWrapper.Wrap<IAsyncResult>((Func<IAsyncResult>) (() => this._httpHandler.BeginProcessRequest(context, cb, extraData)));
			}

			public void EndProcessRequest(IAsyncResult result)
			{
				HttpHandlerUtil.ServerExecuteHttpHandlerWrapper.Wrap((Action) (() => this._httpHandler.EndProcessRequest(result)));
			}
		}
	}
}