namespace MvcKickstart.Infrastructure
{
	public interface IMetricTracker
	{
		#region Guage

		/// <summary>
		/// Captures a series of measurements where each one represents the value under observation at one point in time.  Graph values will remain at the last recorded value until a new value is recorded.
		/// Examples of gauge measurements include the number of concurrent connections, temperature outside, the amount of available disk space
		/// </summary>
		/// <param name="key">Metric key</param>
		/// <param name="value">Value to log</param>
		/// <returns></returns>
		bool Gauge(string key, int value);
		/// <summary>
		/// Captures a series of measurements where each one represents the value under observation at one point in time.  Graph values will remain at the last recorded value until a new value is recorded.
		/// Examples of gauge measurements include the number of concurrent connections, temperature outside, the amount of available disk space
		/// </summary>
		/// <param name="key">Metric key</param>
		/// <param name="value">Value to log</param>
		/// <param name="sampleRate">Percent of packets to send to statsd.  Use this with frequent calls to limit the number of requests to statsd</param>
		/// <returns></returns>
		bool Gauge(string key, int value, double sampleRate);

		#endregion

		#region Value

		/// <summary>
		/// Log an arbitrary value for the specified key.  Examples include number of items returned from a search, etc
		/// </summary>
		/// <param name="key">Metric key</param>
		/// <param name="value">Value to log</param>
		/// <returns></returns>
		bool Value(string key, int value);
		/// <summary>
		/// Log an arbitrary value for the specified key.  Examples include number of items returned from a search, etc
		/// </summary>
		/// <param name="key">Metric key</param>
		/// <param name="value">Value to log</param>
		/// <param name="sampleRate">Percent of packets to send to statsd.  Use this with frequent calls to limit the number of requests to statsd</param>
		/// <returns></returns>
		bool Value(string key, int value, double sampleRate);
		/// <summary>
		/// Log an arbitrary value for the specified key.  Examples include number of items returned from a search, etc
		/// </summary>
		/// <param name="key">Metric key</param>
		/// <param name="value">Value to log</param>
		/// <returns></returns>
		bool Value(string key, long value);
		/// <summary>
		/// Log an arbitrary value for the specified key.  Examples include number of items returned from a search, etc
		/// </summary>
		/// <param name="key">Metric key</param>
		/// <param name="value">Value to log</param>
		/// <param name="sampleRate">Percent of packets to send to statsd.  Use this with frequent calls to limit the number of requests to statsd</param>
		/// <returns></returns>
		bool Value(string key, long value, double sampleRate);
		/// <summary>
		/// Log an arbitrary value for the specified key.  Examples include number of items returned from a search, etc
		/// </summary>
		/// <param name="key">Metric key</param>
		/// <param name="value">Value to log</param>
		/// <returns></returns>
		bool Value(string key, double value);
		/// <summary>
		/// Log an arbitrary value for the specified key.  Examples include number of items returned from a search, etc
		/// </summary>
		/// <param name="key">Metric key</param>
		/// <param name="value">Value to log</param>
		/// <param name="sampleRate">Percent of packets to send to statsd.  Use this with frequent calls to limit the number of requests to statsd</param>
		/// <returns></returns>
		bool Value(string key, double value, double sampleRate);

		#endregion

		#region Timing

		/// <summary>
		/// Log amount of time it took for the specified key
		/// </summary>
		/// <param name="key">Metric key</param>
		/// <param name="value">Time in milliseconds</param>
		/// <returns></returns>
		bool Timing(string key, int value);
		/// <summary>
		/// Log amount of time it took for the specified key
		/// </summary>
		/// <param name="key">Metric key</param>
		/// <param name="value">Time in milliseconds</param>
		/// <param name="sampleRate">Percent of packets to send to statsd.  Use this with frequent calls to limit the number of requests to statsd</param>
		/// <returns></returns>
		bool Timing(string key, int value, double sampleRate);
		/// <summary>
		/// Log amount of time it took for the specified key
		/// </summary>
		/// <param name="key">Metric key</param>
		/// <param name="value">Time in milliseconds</param>
		/// <returns></returns>
		bool Timing(string key, long value);
		/// <summary>
		/// Log amount of time it took for the specified key
		/// </summary>
		/// <param name="key">Metric key</param>
		/// <param name="value">Time in milliseconds</param>
		/// <param name="sampleRate">Percent of packets to send to statsd.  Use this with frequent calls to limit the number of requests to statsd</param>
		/// <returns></returns>
		bool Timing(string key, long value, double sampleRate);
		/// <summary>
		/// Log amount of time it took for the specified key
		/// </summary>
		/// <param name="key">Metric key</param>
		/// <param name="value">Time in milliseconds</param>
		/// <returns></returns>
		bool Timing(string key, double value);
		/// <summary>
		/// Log amount of time it took for the specified key
		/// </summary>
		/// <param name="key">Metric key</param>
		/// <param name="value">Time in milliseconds</param>
		/// <param name="sampleRate">Percent of packets to send to statsd.  Use this with frequent calls to limit the number of requests to statsd</param>
		/// <returns></returns>
		bool Timing(string key, double value, double sampleRate);

		#endregion

		#region Decrement

		/// <summary>
		/// Decrements a counter for the specified key
		/// </summary>
		/// <param name="key">Metric key</param>
		/// <returns></returns>
		bool Decrement(string key);
		/// <summary>
		/// Decrements a counter for the specified key by the specified magnitude
		/// </summary>
		/// <param name="key">Metric key</param>
		/// <param name="magnitude">Decrease counter by this value</param>
		/// <returns></returns>
		bool Decrement(string key, int magnitude);
		/// <summary>
		/// Decrements a counter for the specified key by the specified magnitude
		/// </summary>
		/// <param name="key">Metric key</param>
		/// <param name="magnitude">Decrease counter by this value</param>
		/// <param name="sampleRate">Percent of packets to send to statsd.  Use this with frequent calls to limit the number of requests to statsd</param>
		/// <returns></returns>
		bool Decrement(string key, int magnitude, double sampleRate);
		/// <summary>
		/// Decrements counters for the specified keys
		/// </summary>
		/// <param name="keys">Metric keys</param>
		/// <returns></returns>
		bool Decrement(params string[] keys);
		/// <summary>
		/// Decrements counters for the specified keys by the specified magnitude
		/// </summary>
		/// <param name="keys">Metric keys</param>
		/// <param name="magnitude">Decrease counters by this value</param>
		/// <returns></returns>
		bool Decrement(int magnitude, params string[] keys);
		/// <summary>
		/// Decrements counters for the specified keys by the specified magnitude
		/// </summary>
		/// <param name="keys">Metric keys</param>
		/// <param name="magnitude">Decrease counters by this value</param>
		/// <param name="sampleRate">Percent of packets to send to statsd.  Use this with frequent calls to limit the number of requests to statsd</param>
		/// <returns></returns>
		bool Decrement(int magnitude, double sampleRate, params string[] keys);

		#endregion

		#region Increment

		/// <summary>
		/// Increments a counter for the specified key
		/// </summary>
		/// <param name="key">Metric key</param>
		/// <returns></returns>
		bool Increment(string key);
		/// <summary>
		/// Increments a counter for the specified key by the specified magnitude
		/// </summary>
		/// <param name="key">Metric key</param>
		/// <param name="magnitude">Increase counter by this value</param>
		/// <returns></returns>
		bool Increment(string key, int magnitude);
		/// <summary>
		/// Increments a counter for the specified key by the specified magnitude
		/// </summary>
		/// <param name="key">Metric key</param>
		/// <param name="magnitude">Increase counter by this value</param>
		/// <param name="sampleRate">Percent of packets to send to statsd.  Use this with frequent calls to limit the number of requests to statsd</param>
		/// <returns></returns>
		bool Increment(string key, int magnitude, double sampleRate);
		/// <summary>
		/// Increments counters for the specified keys
		/// </summary>
		/// <param name="keys">Metric keys</param>
		/// <returns></returns>
		bool Increment(params string[] keys);
		/// <summary>
		/// Increments counters for the specified keys by the specified magnitude
		/// </summary>
		/// <param name="keys">Metric keys</param>
		/// <param name="magnitude">Increase counters by this value</param>
		/// <returns></returns>
		bool Increment(int magnitude, params string[] keys);
		/// <summary>
		/// Increments counters for the specified keys by the specified magnitude
		/// </summary>
		/// <param name="keys">Metric keys</param>
		/// <param name="magnitude">Increase counters by this value</param>
		/// <param name="sampleRate">Percent of packets to send to statsd.  Use this with frequent calls to limit the number of requests to statsd</param>
		/// <returns></returns>
		bool Increment(int magnitude, double sampleRate, params string[] keys);

		#endregion
	}
}