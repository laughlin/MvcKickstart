using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MvcKickstart.Analytics.Models.GoogleAnalytics;

namespace MvcKickstart.Analytics.ViewModels.Widgets
{
	public class Config
	{
		[Display(Name = "Analytics Profile")]
		public string ProfileId { get; set; }
		public IList<Account> Accounts { get; set; }
		public IList<Profile> Profiles { get; set; }
	}
}