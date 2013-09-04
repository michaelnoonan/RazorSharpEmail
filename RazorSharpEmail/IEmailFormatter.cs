using System.Net.Mail;
using RazorEngine.Templating;

namespace RazorSharpEmail
{
	public interface IEmailFormatter
	{
        MailMessage BuildMailMessageFrom<TModel>(TModel model, DynamicViewBag viewBag = null);
        TemplatedEmail BuildTemplatedEmailFrom<TModel>(TModel model, DynamicViewBag viewBag = null);
        MailMessage BuildMailMessageFrom(TemplatedEmail templatedEmail, DynamicViewBag viewBag = null);
	    MailMessage BuildMailMessageFrom(TemplatedEmailAfterLayout templatedEmailAfterLayout);
	    TemplatedEmailAfterLayout LayoutTemplatedEmail(TemplatedEmail templatedEmail, DynamicViewBag viewBag = null);
	}
}