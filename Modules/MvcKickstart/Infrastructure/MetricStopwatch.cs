using System;
using System.Diagnostics;
using StructureMap;

namespace MvcKickstart.Infrastructure
{
	/// <summary>
	/// Tracks the amount of time for the specified metric.  Timer will stop when the object is disposed.  The idea is that you will use this in a using statement:
	/// <code>
	/// using (new MetricStopwatch(Metric.Profiling_ResolveRoute)) {
	/// ...
	/// }
	/// </code>
	/// </summary>
	/// <example>
	/// using (new MetricStopwatch(Metric.Profiling_ResolveRoute)) {
	///   // Whatever actions you want to profile
	/// }
	/// </example>
	public class MetricStopwatch : IDisposable
	{
		private Stopwatch _stopwatch;
		private readonly string _metric;

		/// <summary>
		/// Tracks the amount of time for the specified metric.  Timer will stop when the object is disposed
		/// </summary>
		/// <param name="metric">Metric to track</param>
		public MetricStopwatch(string metric)
		{
			_metric = metric;
			_stopwatch = new Stopwatch();
			_stopwatch.Start();
		}

		public void Dispose()
		{
			_stopwatch.Stop();
			var metricTracker = ObjectFactory.GetInstance<IMetricTracker>();
			metricTracker.Timing(_metric, _stopwatch.Elapsed.TotalMilliseconds);
			_stopwatch = null;
		}
	}
}