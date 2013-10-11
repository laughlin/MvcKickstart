using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using CacheStack;
using MvcKickstart.ViewModels;
using ServiceStack.CacheAccess;
using ServiceStack.Logging;

namespace MvcKickstart.Infrastructure
{
	public abstract class ControllerBase : Controller, IWithCacheContext
	{
		protected IDbConnection Db { get; private set; }
		protected IMetricTracker Metrics { get; private set; }
		protected ILog Log { get; private set; }
		protected ICacheClient Cache { get; private set; }
		/// <summary>
		/// Used to set the cache context for donut cached actions
		/// </summary>
		public ICacheContext CacheContext { get; private set; }

		protected ControllerBase()
		{
			Log = LogManager.GetLogger(GetType());
		}

		protected ControllerBase(IDbConnection db) : this()
		{
			Db = db;
		}

		protected ControllerBase(ICacheClient cache) : this()
		{
			Cache = cache;
			CacheContext = new CacheContext(Cache);
		}

		protected ControllerBase(IDbConnection db, ICacheClient cache) : this(cache)
		{
			Db = db;
		}

		protected ControllerBase(IDbConnection db, ICacheClient cache, IMetricTracker metrics) : this(db, cache)
		{
			Metrics = metrics;
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
		/// <returns></returns>
		protected JsonResult JsonError()
		{
			return JsonError(new Error());
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
			if (error.ValidationMessages == null && ModelState != null)
			{
				error.ValidationMessages = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray());
			}

			Response.TrySkipIisCustomErrors = true;
			Response.StatusCode = responseCode;
			return Json(error);
		}

		/// <summary>
		/// Specify a success notification to be shown for this request
		/// </summary>
		/// <param name="message">Notification message</param>
		public void NotifySuccess(string message)
		{
			Notify(message, NotificationType.Success);
		}
		/// <summary>
		/// Specify a info notification to be shown for this request
		/// </summary>
		/// <param name="message">Notification message</param>
		public void NotifyInfo(string message)
		{
			Notify(message, NotificationType.Info);
		}
		/// <summary>
		/// Specify a warning notification to be shown for this request
		/// </summary>
		/// <param name="message">Notification message</param>
		public void NotifyWarning(string message)
		{
			Notify(message, NotificationType.Warning);
		}
		/// <summary>
		/// Specify an error notification to be shown for this request
		/// </summary>
		/// <param name="message">Notification message</param>
		public void NotifyError(string message)
		{
			Notify(message, NotificationType.Error);
		}
		/// <summary>
		/// Specify a notification to be shown for this request
		/// </summary>
		/// <param name="message">Notification message</param>
		/// <param name="type">Notification type</param>
		public void Notify(string message, NotificationType type)
		{
			Notify(new Notification(message, type));
		}
		/// <summary>
		/// Specify a notification to be shown for this request
		/// </summary>
		/// <param name="notification">Notification message</param>
		public void Notify(Notification notification)
		{
			TempData[ViewDataConstants.Notification] = notification;
		}
	}
}