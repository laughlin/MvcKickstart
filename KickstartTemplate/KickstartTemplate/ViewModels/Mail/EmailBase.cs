using System;

namespace KickstartTemplate.ViewModels.Mail
{
	public abstract class EmailBase
	{
		public string To { get; set; }
		public string From { get; set; }
		public string SiteTitle { get; set; }
	}
}