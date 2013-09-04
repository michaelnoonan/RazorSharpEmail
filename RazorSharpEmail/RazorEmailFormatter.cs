using System;
using System.Net.Mail;
using System.Web;
using RazorEngine.Templating;

namespace RazorSharpEmail
{
	public class RazorEmailFormatter : IEmailFormatter
	{
		private readonly ITemplateService _templateService;
		private readonly IEmailTemplateInitializer _emailTemplateInitializer;

		public RazorEmailFormatter(ITemplateService templateService, IEmailTemplateInitializer emailTemplateInitializer)
		{
			_templateService = templateService;
			_emailTemplateInitializer = emailTemplateInitializer;
		}

	    public MailMessage BuildMailMessageFrom<TModel>(TModel model, DynamicViewBag viewBag = null)
	    {
            return BuildMailMessageFrom(BuildTemplatedEmailFrom(model, viewBag));
        }

	    public TemplatedEmail BuildTemplatedEmailFrom<TModel>(TModel model, DynamicViewBag viewBag = null)
		{
	        var language = EnsureCurrentLanguageScope();

	        var templatedEmail = new TemplatedEmail();

            if (viewBag == null) viewBag = new DynamicViewBag();

	        // Parse the email subject and cleanse newline characters (otherwise the email class will throw)
			templatedEmail.Subject = HttpUtility.HtmlDecode(_templateService.Run(_emailTemplateInitializer.GetSubjectTemplateName(typeof(TModel), language), model, viewBag: viewBag)).CleanseCRLF();

			// Parse the body in both formats which will be wrapped in the layouts in a separate step
			templatedEmail.PlainTextBody = HttpUtility.HtmlDecode(_templateService.Run(_emailTemplateInitializer.GetPlainTextTemplateName(typeof(TModel), language), model, viewBag: viewBag));
			templatedEmail.HtmlBody = _templateService.Run(_emailTemplateInitializer.GetHtmlTemplateName(typeof(TModel), language), model, viewBag: viewBag);

			return templatedEmail;
		}

	    public MailMessage BuildMailMessageFrom(TemplatedEmail templatedEmail, DynamicViewBag viewBag = null)
	    {
	        if (viewBag == null) viewBag = new DynamicViewBag();
	        return BuildMailMessageFrom(LayoutTemplatedEmail(templatedEmail, viewBag));
	    }

	    public TemplatedEmailAfterLayout LayoutTemplatedEmail(TemplatedEmail templatedEmail, DynamicViewBag viewBag = null)
        {
            var templatedEmailAfterLayout = new TemplatedEmailAfterLayout();

            if (viewBag == null) viewBag = new DynamicViewBag();

            templatedEmailAfterLayout.Subject = templatedEmail.Subject;
            templatedEmailAfterLayout.PlainText = LayoutPlainTextContent(templatedEmail, viewBag);
            templatedEmailAfterLayout.Html = LayoutHtmlContent(templatedEmail, viewBag);

            return templatedEmailAfterLayout;
        }

	    public MailMessage BuildMailMessageFrom(TemplatedEmailAfterLayout templatedEmailAfterLayout)
        {
			// Create the mail message and set the subject
			var mailMessage = new MailMessage { Subject = templatedEmailAfterLayout.Subject };

			// Create the plain text view
			var plainTextView = AlternateView.CreateAlternateViewFromString(templatedEmailAfterLayout.PlainText, null, "text/plain");

			// Create the html view
			var htmlView = AlternateView.CreateAlternateViewFromString(templatedEmailAfterLayout.Html, null, "text/html");

			// Add the views
			mailMessage.AlternateViews.Add(plainTextView);
			mailMessage.AlternateViews.Add(htmlView);

			return mailMessage;
		}

        private string LayoutPlainTextContent(TemplatedEmail templatedEmail, DynamicViewBag viewBag = null)
		{
			var language = EnsureCurrentLanguageScope();
            return _templateService.Run(_emailTemplateInitializer.GetPlainTextLayoutName(language), templatedEmail.PlainTextBody, viewBag: viewBag);
		}

        private string LayoutHtmlContent(TemplatedEmail templatedEmail, DynamicViewBag viewBag = null)
		{
			var language = EnsureCurrentLanguageScope();
            return _templateService.Run(_emailTemplateInitializer.GetHtmlLayoutName(language), templatedEmail.HtmlBody, viewBag: viewBag);
		}

		private static string EnsureCurrentLanguageScope()
		{
			if (LanguageScope.CurrentLanguage == null)
			{
				throw new InvalidOperationException(
					"There is no current LanguageScope. This is required to determine the precompiled layout to use for wrapping the content of the email.");
			}

			return LanguageScope.CurrentLanguage;
		}
	}
}