using System.Configuration;
using log4net.Appender;
using log4net.Core;

namespace MvcKickstart.Infrastructure
{
	public class MetricAppender : AppenderSkeleton
	{
		public string Increment { get; set; }
		public int Value { get; set; }

		private string Host { get; set; }
		private int Port { get; set; }
		private string Prefix { get; set; }

		public MetricAppender()
		{
			Value = 1;

			// Not able to easily use structuremap at this point, so I'll just create the object myself. 
			Host = ConfigurationManager.AppSettings["Metrics:Host"];
			int port;
			int.TryParse(ConfigurationManager.AppSettings["Metrics:Port"], out port);
			Port = port;
			Prefix = ConfigurationManager.AppSettings["Metrics:Prefix"] ?? string.Empty;
		}

		protected override void Append(LoggingEvent loggingEvent)
		{
			using (var tracker = new MetricTracker(Host, Port, Prefix))
			{
				tracker.Increment(Increment, Value);
			}
		}
	}
}