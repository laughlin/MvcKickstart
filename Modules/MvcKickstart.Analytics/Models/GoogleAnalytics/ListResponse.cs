using System.Collections.Generic;

namespace MvcKickstart.Analytics.Models.GoogleAnalytics
{
	public class ListResponse<T>
	{
		public string Kind { get; set; }
		public string Username { get; set; }
		public int TotalResults { get; set; }
		public int ItemsPerPage { get; set; }
		public string PreviousLink { get; set; }
		public string NextLink { get; set; }
		public IList<T> Items { get; set; }
	}
}