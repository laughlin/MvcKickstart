using System;
using MvcKickstart.Infrastructure.Data.Schema.Attributes;

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

		public int UserId { get; set; }

		public DateTime CreatedOn { get; set; }
	}
}