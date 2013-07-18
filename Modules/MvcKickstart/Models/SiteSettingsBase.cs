using System.ComponentModel.DataAnnotations.Schema;
using Spruce.Schema.Attributes;

namespace MvcKickstart.Models
{
	// The idea is that there will only be one row representing site settings. This class defines the base to start with for modules and applications. 
	// Modules and applications should be able to add properties as needed - every property should be nullable
	[Table("SiteSettings")]
	public abstract class SiteSettingsBase
	{
		[AutoIncrement]
		public int Id { get; set; }
	}
}