namespace MvcKickstart.Infrastructure
{
	public class Notification
	{
		public NotificationType Type { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }

		public string LinkText { get; set; }
		public string LinkUrl { get; set; }
		public string LinkTitle { get; set; }

		public bool ShowLinkBeforeMessage { get; set; }
		public bool ShowLinkInline { get; set; }

		public Notification(string message, NotificationType type)
		{
			Message = message;
			Type = type;
			ShowLinkInline = true;
		}
	}
}