using System.Security.Cryptography;
using System.Text;

namespace MvcKickstart.Infrastructure.Extensions
{
	public static class StringExtensions
	{
		public static string ToMD5Hash(this string input)
		{
			var crypto = MD5.Create();
			var inputBytes = Encoding.UTF8.GetBytes(input);
			var hashBytes = crypto.ComputeHash(inputBytes);

			// Convert byte array to hex string
			var sb = new StringBuilder();
			foreach (var hashByte in hashBytes)
			{
				sb.Append(hashByte.ToString("X2"));
			}
			return sb.ToString();
		}
		public static string ToSHAHash(this string input)
		{
			var crypto = SHA256.Create();
			var inputBytes = Encoding.UTF8.GetBytes(input);
			var hashBytes = crypto.ComputeHash(inputBytes);

			// Convert byte array to hex string
			var sb = new StringBuilder();
			foreach (var hashByte in hashBytes)
			{
				sb.Append(hashByte.ToString("X2"));
			}
			return sb.ToString();
		}
		public static string ToSentence(this string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return string.Empty;

			var newText = new StringBuilder(text.Length * 2);
			newText.Append(text[0]);
			for (var i = 1; i < text.Length; i++)
			{
				if (char.IsUpper(text[i]) && text[i - 1] != ' ')
					newText.Append(' ');
				newText.Append(text[i]);
			}
			return newText.ToString();
		}
	}
}