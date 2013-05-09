using ServiceStack.Text;
using Spruce.Schema;

namespace MvcKickstart.Infrastructure.Data.ScriptedObjects
{
	public class View_AccountWithBalance : View
	{
		public override string Name { get { return "View_AccountWithBalance"; } }
		public override string CreateScript
		{
			get
			{
				return @"
CREATE VIEW [dbo].[{0}]
AS
SELECT 
	a.*, 
	(select IsNull(sum(e.Amount),0) from [Entries] e where e.AccountId=a.Id) as 'Balance'
FROM [Accounts] a".Fmt(Name);
			}
		}
	}
}