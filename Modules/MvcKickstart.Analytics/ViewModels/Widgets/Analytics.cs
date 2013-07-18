using System;
using System.Collections.Generic;
using MvcKickstart.Analytics.Models.GoogleAnalytics;

namespace MvcKickstart.Analytics.ViewModels.Widgets
{
	public class Analytics
	{
		public int Duration { get; set; }
		public TimeSpan AveragePageLoadTime { get; set; }
		public IDictionary<string, int> TopSearches { get; set; }
		public IDictionary<string, int> TopReferrers { get; set; }
		public IDictionary<string, int> PageViews { get; set; }
		public IDictionary<string, string> PageTitles { get; set; }
		public double PercentExitRate { get; set; }
		public double PageviewsPerVisit { get; set; }
		public double EntranceBounceRate { get; set; }
		public TimeSpan AverageTimeOnSite { get; set; }
		public double PercentNewVisits { get; set; }
		public int TotalPageViews { get; set; }
		public int TotalVisits { get; set; }
		public IDictionary<DateTime, int> Visits { get; set; }

		public DateTime Start { get; set; }
		public DateTime End { get; set; }

		public Profile Profile { get; set; }
	}
}