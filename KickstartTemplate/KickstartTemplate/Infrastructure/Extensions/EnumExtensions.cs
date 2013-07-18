using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace KickstartTemplate.Infrastructure.Extensions
{
	public static class EnumExtensions
	{
		private static readonly ConcurrentDictionary<Metric, string> MetricTrackingNames = new ConcurrentDictionary<Metric, string>();
		public static string GetMetricTrackingName(this Metric metric)
		{
			String value;
			if (MetricTrackingNames.TryGetValue(metric, out value))
				return value;

			value = ConfigurationManager.AppSettings["Metrics:Prefix"] ?? "KickstartTemplate.";
			var enumType = metric.GetType();
			var info = enumType.GetField(metric.ToString());
			if (info == null)
				return value + metric;

			var displayAttribute = info.GetCustomAttributes(false).OfType<DisplayAttribute>().FirstOrDefault();
			if (displayAttribute == null)
			{
				value += metric.ToString();
			}
			else
			{
				var name = displayAttribute.GetName() ?? string.Empty;
				var group = displayAttribute.GetGroupName();
				if (!String.IsNullOrEmpty(group))
					value += group + "." + name;
				else
					value += name;

				value = Regex.Replace(value.Replace(':', '.'), @"[^a-zA-Z0-9\-_\.]+", string.Empty);
			}

			MetricTrackingNames.TryAdd(metric, value);

			return value;
		}

		private static readonly ConcurrentDictionary<Metric, string> MetricAliasNames = new ConcurrentDictionary<Metric, string>();
		public static string GetMetricAliasName(this Metric metric)
		{
			String value;
			if (MetricAliasNames.TryGetValue(metric, out value))
				return value;

			var enumType = metric.GetType();
			var info = enumType.GetField(metric.ToString());
			if (info == null)
			{
				value = metric.ToString();
			}
			else
			{
				var displayAttribute = info.GetCustomAttributes(false).OfType<DisplayAttribute>().FirstOrDefault();
				if (displayAttribute == null)
				{
					value = metric.ToString();
				}
				else
				{
					value = displayAttribute.GetName() ?? metric.ToString();
				}
			}
			value = value.ToSentence();

			MetricAliasNames.TryAdd(metric, value);

			return value;
		}
	}
}