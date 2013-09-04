using System;
using ApprovalUtilities.Utilities;

namespace RazorSharpEmail.Tests
{
    public static class TemplatedEmailExtensions
    {
        public static string Everything(this TemplatedEmail email)
        {
            return
                "{1}{0}{2}{0}{3}".FormatWith(
                    Environment.NewLine + "---------------------------------" + Environment.NewLine,
                    email.Subject, email.HtmlBody, email.PlainTextBody);
        }

        public static string Everything(this TemplatedEmailAfterLayout email)
        {
            return
                "{1}{0}{2}{0}{3}".FormatWith(
                    Environment.NewLine + "---------------------------------" + Environment.NewLine,
                    email.Subject, email.Html, email.PlainText);
        }
    }
}
