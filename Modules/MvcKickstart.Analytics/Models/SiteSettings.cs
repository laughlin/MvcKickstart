using System.ComponentModel.DataAnnotations;
using MvcKickstart.Models;

namespace MvcKickstart.Analytics.Models
{
	public class SiteSettings : SiteSettingsBase
	{
		[StringLength(100)]
		public string AnalyticsToken { get; set; }
		[StringLength(50)]
		public string AnalyticsProfileId { get; set; }
	}
}
