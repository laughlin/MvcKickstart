namespace MvcKickstart.Analytics.Models.GoogleAnalytics
{
	public class Profile
	{
		public string Id { get; set; }
		public string Kind { get; set; }
		public string SelfLink { get; set; }
		public string AccountId { get; set; }
		public string WebPropertyId { get; set; }
		public string InternalWebPropertyId { get; set; }
		public string Name { get; set; }
		public string Currency { get; set; }
		public string Timezone { get; set; }
		public string WebsiteUrl { get; set; }
		public string DefaultPage { get; set; }
		public string ExcludeQueryParameters { get; set; }
		public string SiteSearchQueryParameters { get; set; }
		public string SiteSearchCategoryParameters { get; set; }
		public string Type { get; set; }
		public string Created { get; set; }
		public string Updated { get; set; }
		public string ECommerceTracking { get; set; }
		public Link ParentLink { get; set; }
		public Link ChildLink { get; set; }
	}
}