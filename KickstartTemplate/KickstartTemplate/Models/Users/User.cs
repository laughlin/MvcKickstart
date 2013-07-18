using System;
using System.ComponentModel.DataAnnotations;
using Spruce.Schema.Attributes;

namespace KickstartTemplate.Models.Users
{
	public class User
	{
		[AutoIncrement]
		public int Id { get; set; }
		[StringLength(50)]
		public string Username { get; set; }
		[StringLength(64)]
		public string Password { get; set; }

		[StringLength(50)]
		public string Name { get; set; }
		[StringLength(200)]
		public string Email { get; set; }

		public bool IsAdmin { get; set; }

		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public bool IsDeleted { get; set; }
	}
}