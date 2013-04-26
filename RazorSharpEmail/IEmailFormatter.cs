using System.Net.Mail;

namespace RazorSharpEmail
{
	public interface IEmailFormatter
	{
		MailMessage BuildMailMessageFrom<TModel>(TModel model);
		TemplatedEmail BuildTemplatedEmailFrom<TModel>(TModel model);
	    MailMessage BuildMailMessageFrom(TemplatedEmail templatedEmail);
	}
}