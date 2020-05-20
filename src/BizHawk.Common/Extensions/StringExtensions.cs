using System;
using System.Linq;

namespace BizHawk.Common.StringExtensions
{
	public static class StringExtensions
	{
		/// <returns>
		/// <see langword="true"/> if <paramref name="str"/> appears in <paramref name="options"/> (case-insensitive)
		/// </returns>
		public static bool In(this string str, params string[] options) =>
			options.Any(opt => string.Equals(opt, str, StringComparison.InvariantCultureIgnoreCase));

		/// <returns>
		/// <paramref name="str"/> with the last char removed, or the original <paramref name="str"/>
		/// if the last char of <paramref name="str"/> is not <paramref name="suffix"/>
		/// </returns>
		public static string RemoveSuffix(this string str, char suffix) =>
			str.Length != 0 && str[str.Length - 1] == suffix
				? str.Substring(0, str.Length - 1)
				: str;

		/// <returns>
		/// the substring of <paramref name="str"/> before the first occurrence of
		/// <paramref name="delimiter"/>, or <see langword="null"/> if not found
		/// </returns>
		public static string? SubstringBeforeOrNull(this string str, string delimiter)
		{
			var index = str.IndexOf(delimiter);
			return index < 0 ? null : str.Substring(0, index);
		}
	}
}
