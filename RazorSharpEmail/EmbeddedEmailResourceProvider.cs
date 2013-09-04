using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RazorSharpEmail
{
	public class EmbeddedEmailResourceProvider : IEmailResourceProvider
	{
        private const string HtmlLayoutName = "_Layout.Html.cshtml";
        private const string PlainTextLayoutName = "_Layout.PlainText.cshtml";

		private readonly Assembly _resourceAssembly = typeof(EmbeddedEmailResourceProvider).Assembly;

		private readonly string _templateNamespace = typeof(EmbeddedEmailResourceProvider).Namespace + ".RazorTemplates";
		private readonly string _imageNamespace = typeof(EmbeddedEmailResourceProvider).Namespace + ".Images";
		private readonly string[] _supportedLanguages;
		private readonly string[] _candidateImages;

		public EmbeddedEmailResourceProvider(Assembly resourceAssembly, string templateNamespace, string imageNamespace, params string[] supportedLanguages)
		{
			_resourceAssembly = resourceAssembly;
			_templateNamespace = templateNamespace;
			_imageNamespace = imageNamespace;
			_supportedLanguages = supportedLanguages;
			_candidateImages = _resourceAssembly.GetManifestResourceNames().Where(x => x.StartsWith(_imageNamespace)).ToArray();
		}

		private string GetCurrentCultureTemplateNamespace()
		{
			if (LanguageScope.CurrentLanguage == null)
				throw new InvalidOperationException("There is no current LanguageScope. This is required to determine the namespace for Language resources.");

			return _templateNamespace + "." + LanguageScope.CurrentLanguage.Replace("-", "_");
		}

		private string[] GetCandidateTemplates()
		{
			return _resourceAssembly.GetManifestResourceNames().Where(x => x.StartsWith(GetCurrentCultureTemplateNamespace() + ".")).ToArray();
		}

		public string GetTemplate(string name)
		{
			var candidateTemplates = GetCandidateTemplates();
			if (candidateTemplates.Any(x => x.EndsWith(name)) == false)
			{
				throw new Exception("Could not locate an embedded resource in the assembly '{0}' for the email template with the name '{1}' for the language {2}. The template should be placed in the namespace '{3}' and set as an Embedded Resource.  The current email templates are: {4}".FormatWith(_resourceAssembly.FullName, name, LanguageScope.CurrentLanguage, GetCurrentCultureTemplateNamespace(), string.Join(", ", candidateTemplates)));
			}

			var matchingTemplateResourceName = candidateTemplates.Single(x => x.EndsWith(name));

			using (var resourceStream = _resourceAssembly.GetManifestResourceStream(matchingTemplateResourceName))
			using (var reader = new StreamReader(resourceStream))
			{
				return reader.ReadToEnd();
			}
		}

	    public string GetSubjectTemplate(Type modelType)
	    {
            var templateName = GetTemplateNameFromModelType(modelType) + ".Subject.cshtml";
	        return GetTemplate(templateName);
	    }

	    public string GetPlainTextBodyTemplate(Type modelType)
	    {
            var templateName = GetTemplateNameFromModelType(modelType) + ".PlainText.cshtml";
            return GetTemplate(templateName);
        }

	    public string GetHtmlBodyTemplate(Type modelType)
	    {
            var templateName = GetTemplateNameFromModelType(modelType) + ".Html.cshtml";
            return GetTemplate(templateName);
        }

	    public string GetPlainTextLayoutTemplate()
	    {
	        return GetTemplate(PlainTextLayoutName);
	    }

	    public string GetHtmlLayoutTemplate()
	    {
	        return GetTemplate(HtmlLayoutName);
	    }

	    private static string GetTemplateNameFromModelType(Type modelType)
	    {
	        return modelType.Name.Replace("Model", string.Empty);
	    }

	    public Stream GetImageStream(string name)
		{
			if (_candidateImages.Any(x => x.EndsWith(name)) == false)
			{
				throw new Exception("Could not locate an embedded resource for the email image with the name '{0}'. The image should be placed in the namespace '{1}' and set as an Embedded Resource.  The current email images are: {2}".FormatWith(name, _imageNamespace, string.Join(", ", _candidateImages)));
			}

			var matchingImageResourceName = _candidateImages.Single(x => x.EndsWith(name));
			return _resourceAssembly.GetManifestResourceStream(matchingImageResourceName);
		}

		public string[] GetSupportedLanguages()
		{
			return _supportedLanguages;
		}
	}
}