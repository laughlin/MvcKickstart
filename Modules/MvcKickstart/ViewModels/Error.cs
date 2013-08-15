using System.Collections.Generic;

namespace MvcKickstart.ViewModels
{
	public class Error
	{
		public int? ErrorCode { get; set; }
		public string Message { get; set; }
		public IDictionary<string, string[]> ValidationMessages { get; set; }
	}
}