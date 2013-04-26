using Email.Models;
using RazorEngine.Templating;

namespace RazorSharpEmail.Tests
{
    public static class Given
    {
    	private static IEmailFormatter initializedEmailFormatter;

        public static IEmailFormatter AnInitializedEmailFormatter()
        {
			if (initializedEmailFormatter == null)
			{
				var templateService = new TemplateService();
				var embeddedEmailResourceProvider = new EmbeddedEmailResourceProvider(
							typeof(SimpleEmailModel).Assembly,
							"Email.RazorTemplates",
							"Email.Images",
							"en-AU");
				var emailTemplateInitializer = new RazorEmailTemplateInitializer(embeddedEmailResourceProvider, templateService);
				emailTemplateInitializer.CompileTemplatesForTypesInSameNamespaceAs<SimpleEmailModel>();
				initializedEmailFormatter = new RazorEmailFormatter(templateService, emailTemplateInitializer);
			}
        	return initializedEmailFormatter;
        }
    }
}