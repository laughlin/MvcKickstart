using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using StructureMap;
using log4net.Appender;
using log4net.Core;

namespace MvcKickstart.Infrastructure
{
	public class MetricAppender : AppenderSkeleton
	{
		public Metric Increment { get; set; }
		public int Value { get; set; }

		private string Host { get; set; }
		private int Port { get; set; }

		public MetricAppender()
		{
			Value = 1;

			// Not able to easily use structuremap at this point, so I'll just create the object myself. 
			Host = ConfigurationManager.AppSettings["MetricTracking_Host"];
			int port;
			int.TryParse(ConfigurationManager.AppSettings["MetricTracking_Port"], out port);
			Port = port;
		}

		protected override void Append(LoggingEvent loggingEvent)
		{
			using (var tracker = new MetricTracker(Host, Port))
			{
				tracker.Increment(Increment, Value);
			}
		}
	}
}