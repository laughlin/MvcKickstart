using System;
using System.Linq.Expressions;

namespace MvcKickstart.Infrastructure
{
	public static class ExpressionHelper
	{
		public static string GetInputName<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression)
		{
			var body = expression.Body as MemberExpression;
			if (body == null)
			{
				var ubody = (UnaryExpression) expression.Body;
				body = ubody.Operand as MemberExpression;
				if (body == null)
					throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.", expression));
			}
			return body.Member.Name;
			//var propInfo = member.Member as PropertyInfo;
			//if (propInfo == null)
			//    throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.", expression));

			//var sourceType = typeof(TModel);
			//if (sourceType != propInfo.ReflectedType && !sourceType.IsSubclassOf(propInfo.ReflectedType))
			//    throw new ArgumentException(string.Format(
			//        "Expresion '{0}' refers to a property that is not from type {1}.", expression, sourceType));

			//return propInfo.Name;
		}
	}
}