using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcKickstart.Infrastructure.Attributes
{
	public class PasswordStrengthAttribute : RegularExpressionAttribute, IClientValidatable
	{
		//regex pattern modified from http://stackoverflow.com/questions/5142103/need-regex-for-password-strength
		public PasswordStrengthAttribute() : base(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[a-z]).*$") { }

		public override string FormatErrorMessage(string name)
		{
			if (string.IsNullOrEmpty(ErrorMessage))
				return "Password must contain at least an uppercase letter and number";
			return base.FormatErrorMessage(name);
		}
		public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
		{
			return new[] { new ModelClientValidationRegexRule(FormatErrorMessage(metadata.GetDisplayName()), Pattern) };
		}
	}
}