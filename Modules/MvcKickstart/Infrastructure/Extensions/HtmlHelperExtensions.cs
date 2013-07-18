using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace MvcKickstart.Infrastructure.Extensions
{
	public static class HtmlHelperExtensions
	{
		#region TextAreaWithMaxLengthFor
		public static MvcHtmlString TextAreaWithMaxLengthFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
		{
			return htmlHelper.TextAreaWithMaxLengthFor(expression, new RouteValueDictionary());
		}
		public static MvcHtmlString TextAreaWithMaxLengthFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
		{
			return htmlHelper.TextAreaWithMaxLengthFor(expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}
		public static MvcHtmlString TextAreaWithMaxLengthFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
		{
			return htmlHelper.TextAreaWithMaxLengthFor(expression, 0, 0, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}
		public static MvcHtmlString TextAreaWithMaxLengthFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, int rows, int columns, object htmlAttributes)
		{
			return htmlHelper.TextAreaWithMaxLengthFor(expression, rows, columns, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}
		public static MvcHtmlString TextAreaWithMaxLengthFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, int rows, int columns, IDictionary<string, object> htmlAttributes)
		{
			var member = expression.Body as MemberExpression;
			if (member != null && (htmlAttributes == null || !htmlAttributes.ContainsKey("maxlength")))
			{
				if (htmlAttributes == null)
					htmlAttributes = new RouteValueDictionary();

				// Find out if the property has StringLength attribute defined
				var stringLength = member.Member
						.GetCustomAttributes(typeof(StringLengthAttribute), true)
						.FirstOrDefault() as StringLengthAttribute;

				if (stringLength != null)
				{
					htmlAttributes.Add("maxlength", stringLength.MaximumLength);
				}

			}
			if (rows != default(int) && columns != default(int))
				return htmlHelper.TextAreaFor(expression, rows, columns, htmlAttributes);
			return htmlHelper.TextAreaFor(expression, htmlAttributes);
		}
		#endregion

		#region TextBoxWithMaxLengthFor
		public static MvcHtmlString TextBoxWithMaxLengthFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
		{
			return htmlHelper.TextBoxWithMaxLengthFor(expression, null);
		}
		public static MvcHtmlString TextBoxWithMaxLengthFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
		{
			return htmlHelper.TextBoxWithMaxLengthFor(expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}
		public static MvcHtmlString TextBoxWithMaxLengthFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
		{
			var member = expression.Body as MemberExpression;
			if (member != null && (htmlAttributes == null || !htmlAttributes.ContainsKey("maxlength")))
			{
				if (htmlAttributes == null)
					htmlAttributes = new RouteValueDictionary();

				// Find out if the property has StringLength attribute defined
				var stringLength = member.Member
						.GetCustomAttributes(typeof(StringLengthAttribute), true)
						.FirstOrDefault() as StringLengthAttribute;

				if (stringLength != null)
				{
					htmlAttributes.Add("maxlength", stringLength.MaximumLength);
				}

			}
			return htmlHelper.TextBoxFor(expression, htmlAttributes);
		}
		#endregion

		#region PasswordWithMaxLengthFor
		public static MvcHtmlString PasswordWithMaxLengthFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
		{
			return htmlHelper.PasswordWithMaxLengthFor(expression, null);
		}
		public static MvcHtmlString PasswordWithMaxLengthFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
		{
			return htmlHelper.PasswordWithMaxLengthFor(expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}
		public static MvcHtmlString PasswordWithMaxLengthFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
		{
			var member = expression.Body as MemberExpression;
			if (member != null && (htmlAttributes == null || !htmlAttributes.ContainsKey("maxlength")))
			{
				if (htmlAttributes == null)
					htmlAttributes = new RouteValueDictionary();

				// Find out if the property has StringLength attribute defined
				var stringLength = member.Member
						.GetCustomAttributes(typeof(StringLengthAttribute), true)
						.FirstOrDefault() as StringLengthAttribute;

				if (stringLength != null)
				{
					htmlAttributes.Add("maxlength", stringLength.MaximumLength);
				}

			}
			return htmlHelper.PasswordFor(expression, htmlAttributes);
		}
		#endregion

		public static MvcHtmlString DescriptionFor<TModel, TValue>(this HtmlHelper<TModel> self, Expression<Func<TModel, TValue>> expression)
		{
			var metadata = ModelMetadata.FromLambdaExpression(expression, self.ViewData);
			var description = metadata.Description;

			return MvcHtmlString.Create(string.Format(@"<span>{0}</span>", description));
		}

		#region DecodedValidationSummary
		public static MvcHtmlString DecodedValidationSummary(this HtmlHelper html, bool excludePropertyErrors)
		{
			return MvcHtmlString.Create(HttpUtility.HtmlDecode(html.ValidationSummary(excludePropertyErrors).ToHtmlString()));
		}
		public static MvcHtmlString DecodedValidationSummary(this HtmlHelper html, string validationMessage)
		{
			return MvcHtmlString.Create(HttpUtility.HtmlDecode(html.ValidationSummary(validationMessage).ToString()));
		}
		#endregion

		#region Notification

		public static MvcHtmlString Notification(this HtmlHelper html, Notification notification)
		{
			if (notification == null)
				return MvcHtmlString.Empty;

			var div = new TagBuilder("div");
			div.AddCssClass("alert");
			if (notification.Type != NotificationType.Warning)
			{
				div.AddCssClass("alert-" + notification.Type.ToString().ToLower());
			}
			if (!string.IsNullOrWhiteSpace(notification.Title))
			{
				var title = new TagBuilder("h4");
				title.SetInnerText(notification.Title);
				div.InnerHtml = title.ToString(TagRenderMode.Normal);
			}

			var content = HttpUtility.HtmlEncode(notification.Message);
			if (!string.IsNullOrEmpty(notification.LinkUrl))
			{
				var link = new TagBuilder("a");
				link.MergeAttribute("href", notification.LinkUrl);
				if (!string.IsNullOrWhiteSpace(notification.LinkTitle))
				{
					link.MergeAttribute("title", notification.LinkTitle);
				}
				link.SetInnerText(notification.LinkText);

				var linkHtml = link.ToString(TagRenderMode.Normal);
				if (notification.ShowLinkBeforeMessage)
				{
					content = linkHtml + (notification.ShowLinkInline ? " " : "<br /><br />") + content;
				}
				else
				{
					content += (notification.ShowLinkInline ? " " : "<br /><br />") + linkHtml;
				}
			}

			div.InnerHtml += content;
			return new MvcHtmlString(div.ToString(TagRenderMode.Normal));
		}

		#endregion
	}
}