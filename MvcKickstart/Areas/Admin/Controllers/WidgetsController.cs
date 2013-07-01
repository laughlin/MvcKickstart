using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CacheStack;
using CacheStack.DonutCaching;
using Google.GData.Analytics;
using Google.GData.Client;
using MvcKickstart.Areas.Admin.ViewModels.Widgets;
using MvcKickstart.Areas.Admin.ViewModels.Widgets.GoogleAnalytics;
using MvcKickstart.Infrastructure;
using MvcKickstart.Infrastructure.Extensions;
using MvcKickstart.Models;
using MvcKickstart.Services;
using RestSharp;
using ServiceStack.CacheAccess;
using Spruce;

namespace MvcKickstart.Areas.Admin.Controllers
{
	[RouteArea("admin")]
	public class WidgetsController : BaseController
	{
		private readonly ISiteSettingsService _siteSettingsService;
		public WidgetsController(IDbConnection db, IMetricTracker metrics, ICacheClient cache, ISiteSettingsService siteSettingsService) : base(db, metrics, cache)
		{
			_siteSettingsService = siteSettingsService;
		}

		public ActionResult Index()
		{			
			return Redirect(Url.Admin().Home().Index());
		}

		[POST("widgets/analytics", RouteName = "Admin_Widgets_Analytics")]
		[DonutOutputCache]
		public ActionResult Analytics(DateTime? start, DateTime? end)
		{
			CacheContext.InvalidateOn(TriggerFrom.Any<SiteSettings>());

			var settings = _siteSettingsService.GetSettings();

			if (string.IsNullOrEmpty(settings.AnalyticsToken))
			{
				const string scope = "https://www.google.com/analytics/feeds/";
				var next = Url.Absolute(Url.Admin().Home().AuthResponse());
				var auth = new AnalyticsAuthorize
					{
						Url = AuthSubUtil.getRequestUrl(next, scope, false, true)
					};
				return View("AnalyticsAuthorize", auth);
			}

			if (string.IsNullOrEmpty(settings.AnalyticsProfileId))
			{
				var config = GetAccountsAndProfiles(settings);

				return View("AnalyticsConfig", config);
			}

			var model = new Analytics
			            	{
								Start = start ?? DateTime.Today.AddDays(-30),
								End = start ?? DateTime.Now,
			            		Visits = new Dictionary<DateTime, int>(),
								PageViews = new Dictionary<string, int>(),
								PageTitles = new Dictionary<string, string>(),
								TopReferrers = new Dictionary<string, int>(),
								TopSearches = new Dictionary<string, int>()
			            	};
			if (model.Start > model.End)
			{
				var tempDate = model.Start;
				model.Start = model.End;
				model.End = tempDate;
			}

			var authFactory = new GAuthSubRequestFactory("analytics", "MvcKickstart")
								{
									Token = settings.AnalyticsToken
								};
								
			var analytics = new AnalyticsService(authFactory.ApplicationName) { RequestFactory = authFactory };

			var profileId = "ga:" + settings.AnalyticsProfileId;

			// Get from All Visits
			var visits = new DataQuery(profileId, model.Start, model.End)
							{
								Metrics = "ga:visits",
								Dimensions = "ga:date",
								Sort = "ga:date"
							};
			var count = 0;
			foreach (DataEntry entry in analytics.Query(visits).Entries)
			{
				var value = entry.Metrics.First().IntegerValue;

				model.Visits.Add(model.Start.AddDays(count++), value);
			}

			// Get Site Usage
			var siteUsage = new DataQuery(profileId, model.Start, model.End)
							{
								Metrics = "ga:visits,ga:pageviews,ga:percentNewVisits,ga:avgTimeOnSite,ga:entranceBounceRate,ga:exitRate,ga:pageviewsPerVisit,ga:avgPageLoadTime"
							};
			var siteUsageResult = (DataEntry)analytics.Query(siteUsage).Entries.FirstOrDefault();
			if (siteUsageResult != null)
			{
				foreach (var metric in siteUsageResult.Metrics)
				{
					switch (metric.Name)
					{
						case "ga:visits":
							model.TotalVisits = metric.IntegerValue;
							break;
						case "ga:pageviews":
							model.TotalPageViews = metric.IntegerValue;
							break;
						case "ga:percentNewVisits":
							model.PercentNewVisits = metric.FloatValue;
							break;
						case "ga:avgTimeOnSite":
							model.AverageTimeOnSite = TimeSpan.FromSeconds(metric.FloatValue);
							break;
						case "ga:entranceBounceRate":
							model.EntranceBounceRate = metric.FloatValue;
							break;
						case "ga:exitRate":
							model.PercentExitRate = metric.FloatValue;
							break;
						case "ga:pageviewsPerVisit":
							model.PageviewsPerVisit = metric.FloatValue;
							break;
						case "ga:avgPageLoadTime":
							model.AveragePageLoadTime = TimeSpan.FromSeconds(metric.FloatValue);
							break;
					}
				}
			}

			// Get Top Pages
			var topPages = new DataQuery(profileId, model.Start, model.End)
							{
								Metrics = "ga:pageviews",
								Dimensions = "ga:pagePath,ga:pageTitle",
								Sort = "-ga:pageviews",
								NumberToRetrieve = 20
							};
			foreach (DataEntry entry in analytics.Query(topPages).Entries)
			{
				var value = entry.Metrics.First().IntegerValue;
				var url = entry.Dimensions.Single(x => x.Name == "ga:pagePath").Value.ToLowerInvariant();
				var title = entry.Dimensions.Single(x => x.Name == "ga:pageTitle").Value;

				if (!model.PageViews.ContainsKey(url))
					model.PageViews.Add(url, 0);
				model.PageViews[url] += value;

				if (!model.PageTitles.ContainsKey(url))
					model.PageTitles.Add(url, title);
			}

			// Get Top Referrers
			var topReferrers = new DataQuery(profileId, model.Start, model.End)
							{
								Metrics = "ga:visits",
								Dimensions = "ga:source,ga:medium",
								Sort = "-ga:visits",
								Filters = "ga:medium==referral",
								NumberToRetrieve = 5
							};
			foreach (DataEntry entry in analytics.Query(topReferrers).Entries)
			{
				var visitCount = entry.Metrics.First().IntegerValue;
				var source = entry.Dimensions.Single(x => x.Name == "ga:source").Value.ToLowerInvariant();

				model.TopReferrers.Add(source, visitCount);
			}

			// Get Top Searches
			var topSearches = new DataQuery(profileId, model.Start, model.End)
							{
								Metrics = "ga:visits",
								Dimensions = "ga:keyword",
								Sort = "-ga:visits",
								Filters = "ga:keyword!=(not set);ga:keyword!=(not provided)",
								NumberToRetrieve = 5
							};
			foreach (DataEntry entry in analytics.Query(topSearches).Entries)
			{
				var visitCount = entry.Metrics.First().IntegerValue;
				var source = entry.Dimensions.Single(x => x.Name == "ga:keyword").Value.ToLowerInvariant();

				model.TopSearches.Add(source, visitCount);
			}

			return View(model);
		}

		private AnalyticsConfig GetAccountsAndProfiles(SiteSettings settings)
		{
			// Using RestSharp because I could not figure out how to access the analytics management api with the analytics nuget
			var client = new RestClient();
			client.AddHandler("application/json", new RestSharpJsonSerializer());
			var accountsRequest = new RestRequest("https://www.googleapis.com/analytics/v3/management/accounts?access_token=" + Server.UrlEncode(settings.AnalyticsToken), Method.GET)
				{
					RequestFormat = DataFormat.Json,
					JsonSerializer = new RestSharpJsonSerializer()
				};
			var profilesRequest = new RestRequest("https://www.googleapis.com/analytics/v3/management/accounts/~all/webproperties/~all/profiles?access_token=" + Server.UrlEncode(settings.AnalyticsToken), Method.GET)
				{
					RequestFormat = DataFormat.Json,
					JsonSerializer = new RestSharpJsonSerializer()
				};
			var accountsResult = client.Execute<ListResponse<Account>>(accountsRequest);
			var profilesResult = client.Execute<ListResponse<Profile>>(profilesRequest);
			var config = new AnalyticsConfig
				{
					Accounts = accountsResult.Data.Items,
					Profiles = profilesResult.Data.Items
				};
			return config;
		}

		[POST("widgets/analytics/config", RouteName = "Admin_Widgets_AnalyticsConfig")]
		public ActionResult AnalyticsConfig(AnalyticsConfig model)
		{
			var settings = _siteSettingsService.GetSettings();
			var config = GetAccountsAndProfiles(settings);
			
			var profile = config.Profiles.SingleOrDefault(x => x.Id == model.ProfileId);
			if (profile == null)
				throw new Exception("Unable to find the specified analytics profile: " + model.ProfileId);

			settings.AnalyticsProfileId = profile.Id;
			Db.Save(settings);
			Cache.Trigger(TriggerFor.Id<SiteSettings>(settings.Id));

			return RedirectToRoute("Admin_Home_Index");
		}
	}
}