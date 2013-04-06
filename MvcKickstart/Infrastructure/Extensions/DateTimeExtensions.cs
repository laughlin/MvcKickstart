using System;

namespace MvcKickstart.Infrastructure.Extensions
{
	public static class DateTimeExtensions
	{
		public static DateTime ToUtc(this DateTime input, string timezoneId)
		{
			return input.ToUtc(TimeZoneInfo.FindSystemTimeZoneById(timezoneId));
		}

		public static DateTime ToUtc(this DateTime input, TimeZoneInfo timezone)
		{
			if (input.Kind == DateTimeKind.Utc)
				return input;

			return TimeZoneInfo.ConvertTimeToUtc(input, timezone);
		}
	}
}