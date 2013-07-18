using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace MvcKickstart.Infrastructure
{
	public class MetricTracker : IDisposable, IMetricTracker
	{
		private readonly UdpClient _udpClient;
		private readonly Random _random = new Random();

		public MetricTracker(string host, int port)
		{
			try
			{
				if (!string.IsNullOrEmpty(host))
					_udpClient = new UdpClient(host, port);
			}
			catch
			{
				// Any failures creating the udpclient should not affect the rest of the site
			}
		}

		#region Guage
		public bool Gauge(string key, int value)
		{
			return Gauge(key, value, 1.0);
		}

		public bool Gauge(string key, int value, double sampleRate)
		{
			return Send(sampleRate, String.Format("{0}:{1:d}|g", key, value));
		}
		#endregion

		#region Value
		public bool Value(string key, int value)
		{
			return Value(key, value, 1.0);
		}
		public bool Value(string key, int value, double sampleRate)
		{
			return Timing(key, value, sampleRate);
		}
		public bool Value(string key, long value)
		{
			return Value(key, value, 1.0);
		}
		public bool Value(string key, long value, double sampleRate)
		{
			return Timing(key, value, sampleRate);
		}
		public bool Value(string key, double value)
		{
			return Value(key, value, 1.0);
		}
		public bool Value(string key, double value, double sampleRate)
		{
			return Timing(key, value, sampleRate);
		}
		#endregion
		
		#region Timing
		public bool Timing(string key, int value)
		{
			return Timing(key, value, 1.0);
		}
		public bool Timing(string key, int value, double sampleRate)
		{
			return Send(sampleRate, String.Format("{0}:{1:d}|ms", key, value));
		}
		public bool Timing(string key, long value)
		{
			return Timing(key, value, 1.0);
		}
		public bool Timing(string key, long value, double sampleRate)
		{
			return Send(sampleRate, String.Format("{0}:{1:d}|ms", key, value));
		}
		public bool Timing(string key, double value)
		{
			return Timing(key, value, 1.0);
		}
		public bool Timing(string key, double value, double sampleRate)
		{
			return Send(sampleRate, String.Format("{0}:{1}|ms", key, value));
		}
		#endregion

		#region Decrement
		public bool Decrement(string key)
		{
			return Increment(key, -1, 1.0);
		}
		public bool Decrement(string key, int magnitude)
		{
			return Decrement(key, magnitude, 1.0);
		}
		public bool Decrement(string key, int magnitude, double sampleRate)
		{
			magnitude = magnitude < 0 ? magnitude : -magnitude;
			return Increment(key, magnitude, sampleRate);
		}
		public bool Decrement(params string[] keys)
		{
			return Increment(-1, 1.0, keys);
		}
		public bool Decrement(int magnitude, params string[] keys)
		{
			magnitude = magnitude < 0 ? magnitude : -magnitude;
			return Increment(magnitude, 1.0, keys);
		}
		public bool Decrement(int magnitude, double sampleRate, params string[] keys)
		{
			magnitude = magnitude < 0 ? magnitude : -magnitude;
			return Increment(magnitude, sampleRate, keys);
		}
		#endregion

		#region Increment
		public bool Increment(string key)
		{
			return Increment(key, 1, 1.0);
		}
		public bool Increment(string key, int magnitude)
		{
			return Increment(key, magnitude, 1.0);
		}
		public bool Increment(string key, int magnitude, double sampleRate)
		{
			var stat = String.Format("{0}:{1}|c", key, magnitude);
			return Send(stat, sampleRate);
		}
		public bool Increment(params string[] keys)
		{
			return Increment(1, 1.0, keys);
		}
		public bool Increment(int magnitude, params string[] keys)
		{
			return Increment(magnitude, 1.0, keys);
		}
		public bool Increment(int magnitude, double sampleRate, params string[] keys)
		{
			return Send(sampleRate, keys.Select(key => String.Format("{0}:{1}|c", key, magnitude)).ToArray());
		}
		#endregion

		protected bool Send(String stat, double sampleRate)
		{
			return Send(sampleRate, stat);
		}

		protected bool Send(double sampleRate, params string[] stats)
		{
			var retval = false; // didn't send anything
			if (sampleRate < 1.0)
			{
				foreach (var stat in stats)
				{
					if (_random.NextDouble() > sampleRate) 
						continue;

					var statFormatted = String.Format("{0}|@{1:f}", stat, sampleRate);
					if (DoSend(statFormatted))
					{
						retval = true;
					}
				}
			}
			else
			{
				foreach (var stat in stats)
				{
					if (DoSend(stat))
					{
						retval = true;
					}
				}
			}

			return retval;
		}

		protected bool DoSend(string stat)
		{
			var data = Encoding.Default.GetBytes(stat + "\n");

			if (_udpClient == null)
			{
				return false;
			}
			try
			{
				_udpClient.SendAsync(data, data.Length);
			}
			catch (Exception ex)
			{
				// TODO: Log this after a while? Handle it better?
			}
			return true;
		}

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				if (_udpClient != null)
				{
					_udpClient.Close();
				}
			}
			catch
			{
			}
		}

		#endregion
	}
}