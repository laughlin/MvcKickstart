using System.Data;
using System.Net;
using System.Text;
using System.Web.Mvc;
using CacheStack;
using MvcKickstart.ViewModels;
using ServiceStack.CacheAccess;
using ServiceStack.Logging;

namespace MvcKickstart.Infrastructure
{
	public abstract class BaseController : Controller, IWithCacheContext
	{
		protected IDbConnection Db { get; private set; }
		protected IMetricTracker Metrics { get; private set; }
		protected ILog Log { get; private set; }
		protected ICacheClient Cache { get; private set; }
		/// <summary>
		/// Used to set the cache context for donut cached actions
		/// </summary>
		public ICacheContext CacheContext { get; private set; }

		protected BaseController(IDbConnection db, IMetricTracker metrics, ICacheClient cache)
		{
			Db = db;
			Metrics = metrics;
			Log = LogManager.GetLogger(GetType());
			Cache = cache;
			CacheContext = new CacheContext(Cache);
		}

		protected override void Execute(System.Web.Routing.RequestContext requestContext)
		{
			base.Execute(requestContext);
			// If this is an ajax request, clear the tempdata notification.
			if (requestContext.HttpContext.Request.IsAjaxRequest())
			{
				TempData[ViewDataConstants.Notification] = null;
			}
		}

		protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
		{
			return new ServiceStackJsonResult
			{
				Data = data,
				ContentType = contentType,
				ContentEncoding = contentEncoding
			};
		}

		/// <summary>
		/// Returns the specified error object as json.  Sets the response status code to the ErrorCode value
		/// </summary>
		/// <param name="error">Error to return</param>
		/// <returns></returns>
		protected JsonResult JsonError(Error error)
		{
			return JsonError(error, error.ErrorCode ?? (int) HttpStatusCode.InternalServerError);
		}
		/// <summary>
		/// Returns the specified error object as json.
		/// </summary>
		/// <param name="error">Error to return</param>
		/// <param name="responseCode">StatusCode to return with the response</param>
		/// <returns></returns>
		protected JsonResult JsonError(Error error, int responseCode)
		{
			Response.StatusCode = responseCode;
			return Json(error);
		}

		/// <summary>
		/// Specify a success notification to be shown this request
		/// </summary>
		/// <param name="message">Notification message</param>
		protected void NotifySuccess(string message)
		{
			Notify(message, NotificationType.Success);
		}
		/// <summary>
		/// Specify a info notification to be shown this request
		/// </summary>
		/// <param name="message">Notification message</param>
		protected void NotifyInfo(string message)
		{
			Notify(message, NotificationType.Info);
		}
		/// <summary>
		/// Specify a warning notification to be shown this request
		/// </summary>
		/// <param name="message">Notification message</param>
		protected void NotifyWarning(string message)
		{
			Notify(message, NotificationType.Warning);
		}
		/// <summary>
		/// Specify an error notification to be shown this request
		/// </summary>
		/// <param name="message">Notification message</param>
		protected void NotifyError(string message)
		{
			Notify(message, NotificationType.Error);
		}
		/// <summary>
		/// Specify a notification to be shown this request
		/// </summary>
		/// <param name="message">Notification message</param>
		/// <param name="type">Notification type</param>
		protected void Notify(string message, NotificationType type)
		{
			TempData[ViewDataConstants.Notification] = new Notification(message, type);
		}
	}
}