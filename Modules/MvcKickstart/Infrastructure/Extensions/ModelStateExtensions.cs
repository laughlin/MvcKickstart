using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace MvcKickstart.Infrastructure.Extensions
{
	public static class ModelStateExtensions
	{
		public static void AddModelErrorFor<TModel>(this ModelStateDictionary dictionary, Expression<Func<TModel, object>> property, string message)
		{
			var name = ExpressionHelper.GetInputName(property);

			dictionary.AddModelError(name, message);
		}
	}
}