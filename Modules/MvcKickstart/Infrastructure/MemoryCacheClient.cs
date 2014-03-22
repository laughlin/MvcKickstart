using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.CacheAccess;
using ServiceStack.Logging;
using ServiceStack.Text;

namespace MvcKickstart.Infrastructure
{
	public class MemoryCacheClient : ICacheClient, ICacheClientBroadcaster
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(MemoryCacheClient));

		private ConcurrentDictionary<string, CacheEntry> _memory;
		private ConcurrentDictionary<string, int> _counters;
		private readonly IList<string> _broadcastNodes;

		public bool FlushOnDispose { get; set; }

		private class CacheEntry
		{
			private object _cacheValue;

			public CacheEntry(object value, DateTime expiresAt)
			{
				Value = value;
				ExpiresAt = expiresAt;
				LastModifiedTicks = DateTime.Now.Ticks;
			}

			internal DateTime ExpiresAt { get; set; }

			internal object Value
			{
				get { return _cacheValue; }
				set
				{
					_cacheValue = value;
					LastModifiedTicks = DateTime.Now.Ticks;
				}
			}

			internal long LastModifiedTicks { get; private set; }
		}

		public MemoryCacheClient()
		{
			_memory = new ConcurrentDictionary<string, CacheEntry>();
			_counters = new ConcurrentDictionary<string, int>();
			_broadcastNodes = new List<string>();
			
			try
			{
				// Expected a list of machine names like bia-web1,bia-web2
				foreach (var node in (ConfigurationManager.AppSettings["CacheStack:BroadcastNodes"] ?? string.Empty).Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					Log.InfoFormat("Parsing cache broadcast node: {0}", node);
					// Don't broadcast to yourself
					if (Environment.MachineName.Equals(node, StringComparison.OrdinalIgnoreCase))
					{
						Log.InfoFormat("Ignoring local machine cache broadcast node: ", node);
						continue;
					}
					var url = node;
					if (!url.StartsWithIgnoreCase("http"))
						url = "http://" + url;
					Log.InfoFormat("Adding cache broadcast url: {0}", url);
					_broadcastNodes.Add(url);
				}
				// Handle list of sites and machine name maps: bia-web1:web1.biacreations.com,bia-web2:web2.biacreations.com
				foreach (var map in (ConfigurationManager.AppSettings["CacheStack:BroadcastNodeMap"] ?? string.Empty).Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					Log.InfoFormat("Parsing cache broadcast node map: {0}", map);
					var mapParts = map.Split(new [] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
					// Don't broadcast to yourself
					if (mapParts.Length != 2)
					{
						Log.Warn("Map format seems incorrect. Expected <computer-name>:<url>");
						continue;
					}
					if (Environment.MachineName.Equals(mapParts[0], StringComparison.OrdinalIgnoreCase))
					{
						Log.InfoFormat("Ignoring local machine cache broadcast node: ", map);
						continue;
					}
					var url = mapParts[1];
					if (!url.StartsWithIgnoreCase("http"))
						url = "http://" + url;
					Log.InfoFormat("Adding cache broadcast url: {0}", url);

					_broadcastNodes.Add(mapParts[1]);
				}
			}
			catch (Exception ex)
			{
				// We don't want to disable the entire app if the cluster caching invalidation doesn't init properly. Just log and continue
				Log.Error("Error parsing cache broadcast node information", ex);
			}
		}

		private bool CacheAdd(string key, object value)
		{
			return CacheAdd(key, value, DateTime.MaxValue);
		}

		private bool TryGetValue(string key, out CacheEntry entry)
		{
			return _memory.TryGetValue(key, out entry);
		}

		private void Set(string key, CacheEntry entry)
		{
			_memory[key] = entry;
		}

		/// <summary>
		/// Stores The value with key only if such key doesn't exist at the server yet. 
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <param name="expiresAt">The expires at.</param>
		/// <returns></returns>
		private bool CacheAdd(string key, object value, DateTime expiresAt)
		{
			CacheEntry entry;
			if (TryGetValue(key, out entry)) return false;

			entry = new CacheEntry(value, expiresAt);
			Set(key, entry);

			return true;
		}

		private bool CacheSet(string key, object value)
		{
			return CacheSet(key, value, DateTime.MaxValue);
		}

		private bool CacheSet(string key, object value, DateTime expiresAt)
		{
			return CacheSet(key, value, expiresAt, null);
		}

		/// <summary>
		/// Adds or replaces the value with key. 
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <param name="expiresAt">The expires at.</param>
		/// <param name="checkLastModified">The check last modified.</param>
		/// <returns>True; if it succeeded</returns>
		private bool CacheSet(string key, object value, DateTime expiresAt, long? checkLastModified)
		{
			CacheEntry entry;
			if (!TryGetValue(key, out entry))
			{
				entry = new CacheEntry(value, expiresAt);
				Set(key, entry);
				return true;
			}

			if (checkLastModified.HasValue
				&& entry.LastModifiedTicks != checkLastModified.Value) return false;

			entry.Value = value;
			entry.ExpiresAt = expiresAt;

			return true;
		}

		private bool CacheReplace(string key, object value)
		{
			return CacheReplace(key, value, DateTime.MaxValue);
		}

		private bool CacheReplace(string key, object value, DateTime expiresAt)
		{
			return !CacheSet(key, value, expiresAt);
		}

		public void Dispose()
		{
			if (!FlushOnDispose) return;

			_memory = new ConcurrentDictionary<string, CacheEntry>();
			_counters = new ConcurrentDictionary<string, int>();
		}

		public bool Remove(string key)
		{
			return Remove(key, true);
		}
		public bool Remove(string key, bool broadcast)
		{
			CacheEntry item;
			var result = _memory.TryRemove(key, out item);
			if (broadcast)
			{
				Broadcast("remove", new { key });
			}
			return result;
		}

		public void RemoveAll(IEnumerable<string> keys)
		{
			foreach (var key in keys)
			{
				try
				{
					Remove(key);
				}
				catch (Exception ex)
				{
					Log.Error(string.Format("Error trying to remove {0} from the cache", key), ex);
				}
			}
		}

		public object Get(string key)
		{
			long lastModifiedTicks;
			return Get(key, out lastModifiedTicks);
		}

		public object Get(string key, out long lastModifiedTicks)
		{
			lastModifiedTicks = 0;

			CacheEntry cacheEntry;
			if (_memory.TryGetValue(key, out cacheEntry))
			{
				if (cacheEntry.ExpiresAt < DateTime.Now)
				{
					_memory.TryRemove(key, out cacheEntry);
					return null;
				}
				lastModifiedTicks = cacheEntry.LastModifiedTicks;
				return cacheEntry.Value;
			}
			return null;
		}

		public T Get<T>(string key)
		{
			var value = Get(key);
			if (value != null) return (T) value;
			return default(T);
		}

		private int UpdateCounter(string key, int value)
		{
			if (!_counters.ContainsKey(key))
			{
				_counters[key] = 0;
			}
			_counters[key] += value;
			return _counters[key];
		}

		public long Increment(string key, uint amount)
		{
			return UpdateCounter(key, 1);
		}

		public long Decrement(string key, uint amount)
		{
			return UpdateCounter(key, -1);
		}

		public bool Add<T>(string key, T value)
		{
			return CacheAdd(key, value);
		}

		public bool Set<T>(string key, T value)
		{
			return CacheSet(key, value);
		}

		public bool Replace<T>(string key, T value)
		{
			return CacheReplace(key, value);
		}

		public bool Add<T>(string key, T value, DateTime expiresAt)
		{
			return CacheAdd(key, value, expiresAt);
		}

		public bool Set<T>(string key, T value, DateTime expiresAt)
		{
			return CacheSet(key, value, expiresAt);
		}

		public bool Replace<T>(string key, T value, DateTime expiresAt)
		{
			return CacheReplace(key, value, expiresAt);
		}

		public bool Add<T>(string key, T value, TimeSpan expiresIn)
		{
			return CacheAdd(key, value, DateTime.Now.Add(expiresIn));
		}

		public bool Set<T>(string key, T value, TimeSpan expiresIn)
		{
			return CacheSet(key, value, DateTime.Now.Add(expiresIn));
		}

		public bool Replace<T>(string key, T value, TimeSpan expiresIn)
		{
			return CacheReplace(key, value, DateTime.Now.Add(expiresIn));
		}

		public void FlushAll()
		{
			FlushAll(true);
		}

		public void FlushAll(bool broadcast)
		{
			_memory = new ConcurrentDictionary<string, CacheEntry>();

			if (broadcast)
			{
				Broadcast("flushall");
			}
		}

		private void Broadcast(string action, object values = null)
		{
			if (!_broadcastNodes.Any())
				return;

			var additionalValues = new StringBuilder();
			if (values != null)
			{
				foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values))
				{
					additionalValues.Append("&");
					additionalValues.Append(descriptor.Name);
					additionalValues.Append("=");
					additionalValues.Append(descriptor.GetValue(values));
				}
			}

			var httpOptions = new HttpClientHandler
				{
					AllowAutoRedirect = true
				};
			foreach (var node in _broadcastNodes)
			{
				var client = new HttpClient(httpOptions);
				client.GetAsync("{0}/Cache.axd?action={1}{2}".Fmt(
					node,
					action,
					additionalValues.ToString()
				)).Start();
			}
		}

		public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
		{
			var valueMap = new Dictionary<string, T>();
			foreach (var key in keys)
			{
				var value = Get<T>(key);
				valueMap[key] = value;
			}
			return valueMap;
		}

		public void SetAll<T>(IDictionary<string, T> values)
		{
			foreach (var entry in values)
			{
				Set(entry.Key, entry.Value);
			}
		}
	}
}