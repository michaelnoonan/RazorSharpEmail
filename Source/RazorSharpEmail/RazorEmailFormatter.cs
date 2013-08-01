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

		public ILogger Logger { get; set; }

		public RazorEmailFormatter(ITemplateService templateService, IEmailTemplateInitializer emailTemplateInitializer)
		{
			_templateService = templateService;
			_emailTemplateInitializer = emailTemplateInitializer;
			Logger = NullLogger.Instance;
		}

		public MailMessage BuildMailMessageFrom<TModel>(TModel model)
		{
			return BuildMailMessageFrom(BuildTemplatedEmailFrom(model));
		}

		public TemplatedEmail BuildTemplatedEmailFrom<TModel>(TModel model)
		{
			Logger.Info(() => "Templating email for {0}".FormatWith(typeof(TModel).Name));

			var language = EnsureCurrentLanguageScope();

			var templatedEmail = new TemplatedEmail();

			// Parse the email subject and cleanse newline characters (otherwise the email class will throw)
			templatedEmail.Subject = HttpUtility.HtmlDecode(_templateService.Run(_emailTemplateInitializer.GetSubjectTemplateName(typeof(TModel), language), model, viewBag: null)).CleanseCRLF();

			// Parse the body in both formats which will be wrapped in the layouts in a separate step
			templatedEmail.PlainTextBody = HttpUtility.HtmlDecode(_templateService.Run(_emailTemplateInitializer.GetPlainTextTemplateName(typeof(TModel), language), model, viewBag: null));
			templatedEmail.HtmlBody = _templateService.Run(_emailTemplateInitializer.GetHtmlTemplateName(typeof(TModel), language), model, viewBag: null);

			return templatedEmail;
		}

		public MailMessage BuildMailMessageFrom(TemplatedEmail templatedEmail)
		{
			Logger.Info(() => "Constructing MailMessage with subject '{0}'".FormatWith(templatedEmail.Subject));

			// Create the mail message and set the subject
			var mailMessage = new MailMessage { Subject = templatedEmail.Subject };

			// Create the plain text view
			var plainTextAfterLayout = LayoutPlainTextContent(templatedEmail);
			var plainTextView = AlternateView.CreateAlternateViewFromString(plainTextAfterLayout, null, "text/plain");

			// Create the html view
			var htmlTextAfterLayout = LayoutHtmlContent(templatedEmail);
			var htmlView = AlternateView.CreateAlternateViewFromString(htmlTextAfterLayout, null, "text/html");

			// Add the views
			mailMessage.AlternateViews.Add(plainTextView);
			mailMessage.AlternateViews.Add(htmlView);

			return mailMessage;
		}

		private string LayoutPlainTextContent(TemplatedEmail templatedEmail)
		{
			var language = EnsureCurrentLanguageScope();
			return _templateService.Run(_emailTemplateInitializer.GetPlainTextLayoutName(language), templatedEmail.PlainTextBody, viewBag: null);
		}

		private string LayoutHtmlContent(TemplatedEmail templatedEmail)
		{
			var language = EnsureCurrentLanguageScope();
			return _templateService.Run(_emailTemplateInitializer.GetHtmlLayoutName(language), templatedEmail.HtmlBody, viewBag: null);
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