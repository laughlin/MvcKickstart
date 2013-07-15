using System.Web;
using System.Web.Mvc;
using ServiceStack.Logging;
using ServiceStack.Logging.Log4Net;

namespace MvcKickstart
{
	public class LoggingConfig
	{
		public static void Initialize()
		{
			LogManager.LogFactory = new Log4NetFactory(true);
		}
	}
}