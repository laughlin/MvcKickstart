using System;
using Spruce.Schema.Attributes;

namespace MvcKickstart.Models.Users
{
	public class PasswordRetrieval
	{
		public PasswordRetrieval() { }
		public PasswordRetrieval(User user)
		{
			UserId = user.Id;
		}
		public PasswordRetrieval(User user, Guid token) : this(user)
		{
			Token = token;
		}

		[AutoIncrement]
		public int Id { get; set; }

		public Guid Token { get; set; }

//		[References(typeof(User))]
		public int UserId { get; set; }

		public DateTime CreatedOn { get; set; }
	}
}