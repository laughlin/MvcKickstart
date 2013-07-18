namespace MvcKickstart.Analytics.Models.GoogleAnalytics
{
	public class Account
	{
		public string Id { get; set; }
		public string Kind { get; set; }
		public string SelfLink { get; set; }
		public string Name { get; set; }
		public string Created { get; set; }
		public string Updated { get; set; }
		public Link ChildLink { get; set; }
	}
}