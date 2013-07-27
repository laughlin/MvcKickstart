using ServiceStack.Logging;
using ServiceStack.Logging.Log4Net;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof($rootnamespace$.LoggingConfig), "PreStart")]

namespace $rootnamespace$ 
{
	public static class LoggingConfig
	{
		public static void PreStart() 
		{
			LogManager.LogFactory = new Log4NetFactory(true);
		}
	}
}
