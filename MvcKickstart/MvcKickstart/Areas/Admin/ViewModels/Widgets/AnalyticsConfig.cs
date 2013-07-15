using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MvcKickstart.Areas.Admin.ViewModels.Widgets.GoogleAnalytics;

namespace MvcKickstart.Areas.Admin.ViewModels.Widgets
{
	public class AnalyticsConfig
	{
		[Display(Name = "Analytics Profile")]
		public string ProfileId { get; set; }
		public IList<Account> Accounts { get; set; }
		public IList<Profile> Profiles { get; set; }
	}
}