using System;

namespace RazorSharpEmail
{
	internal static class StringExtensions
	{
		public static string FormatWith(this string value, params object[] values)
		{
			return String.Format(value, values);
		}

		public static string CleanseCRLF(this string thisObject)
		{
			return thisObject.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
		}
	}
}