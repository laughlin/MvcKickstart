using System;
using System.Collections.Generic;
using System.Threading;
using log4net.Appender;
using log4net.Core;

namespace MvcKickstart.Infrastructure
{
	/// <summary>
	///         Caching version of the standard log4net SmtpAppender. This appender will
	///         cache log events that are to be sent out via smtp and send them in block.
	/// 
	///         Configuration options:
	/// 
	///         <FlushInterval value="hh:mm:ss" />
	///               Indicates the periodic interval for log events flushing (e.g. sending and e-mail), specified
	///               as a time span.  If the value of FlushInterval is 0:0:0 (zero), periodic interval flushing
	///               is surpressed.  Default value:  00:00:05 (5 seconds).
	/// 
	///         <FlushCount value="x" />
	///               Indicates the number of log events received by the appender which will trigger flushing
	///               (e.g. sending and e-mail).  If the value FlushCount is 0, buffer flush triggering based
	///               the number of log events received is surpressed. Default value:  0
	/// 
	///         <MaxBufferSize value="x"/>
	///               Maximum number of log events to send with the smtp message.  
	///               If more than MaxBufferSize events are received before the flushing
	///               criteria is met, only the newest MaxBufferSize events are saved in the buffer.  A value
	///               of 0 indicates no limit in the size of the cache.  Default value:  0 (no limit)
	/// 
	///         Sample SmtpCachingAppender configuration (in addition to all standard SmtpAppender options).
	///               Note the namespace and assembly name for the appender.
	/// 
	///         <appender name="SmtpCachingAppender" type="MvcKickstart.Infrastructure.SmtpCachingAppender, MvcKickstart">
	///               . . .
	///               <FlushInterval value="00:05:00" />
	///               <FlushCount value="20" />
	///               <MaxBufferSize value="3"/>
	///         </appender>
	/// </summary>
	public class SmtpCachingAppender : SmtpAppender
	{
		/// <summary>
		/// Maximum period of time elapsed before sending any cached log events
		/// </summary>
		public TimeSpan FlushInterval { get; set; }
		/// <summary>
		/// Maximum  number of smtp events to cache before sending via smtp.
		/// </summary>
		public int FlushCount { get; set; }
		/// <summary>
		/// Maximum number of events to send via smtp.
		/// </summary>
		public int MaxBufferSize { get; set; }

		private int _cachedMessagesCount;
		private bool _isTimedFlush;
		private Timer _timer;
		private List<LoggingEvent> _loggingEventsCache = new List<LoggingEvent>();

		public SmtpCachingAppender()
		{
			FlushInterval = new TimeSpan(0, 0, 5);
		}

		public override void ActivateOptions()
		{
			if (FlushInterval > TimeSpan.Zero)
			{
				// Create a timer that fires to force flushing log events via smtp at specified interval
				_timer = new Timer((Object stateInfo) =>
					{
						_isTimedFlush = true;
						Flush(true);
					}, null, FlushInterval, FlushInterval);
			}
			base.ActivateOptions();
		}

		protected override void SendBuffer(LoggingEvent[] events)
		{
			_loggingEventsCache.AddRange(events);

			if (MaxBufferSize > 0)
			{
				// Check to see if the number of events in our cache are over the MaxBufferSize limit. If so, purge the older events
				var extraEvents = _loggingEventsCache.Count - MaxBufferSize;
				if (extraEvents > 0 && extraEvents <= _loggingEventsCache.Count)
				{
					_loggingEventsCache.RemoveRange(0, extraEvents);
				}
			}
			_cachedMessagesCount++;
			if ((FlushCount > 0 && _cachedMessagesCount >= FlushCount) || _isTimedFlush)
			{
				if (_loggingEventsCache.Count > 0)
				{
					base.SendBuffer(_loggingEventsCache.ToArray());
					_loggingEventsCache.Clear();
				}
				_isTimedFlush = false;
				_cachedMessagesCount = 0;
			}
		}
	}
}
