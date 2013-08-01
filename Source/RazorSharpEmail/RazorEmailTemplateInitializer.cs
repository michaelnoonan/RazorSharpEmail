using System;
using System.Linq;
using RazorEngine.Templating;

namespace RazorSharpEmail
{
	public class RazorEmailTemplateInitializer : IEmailTemplateInitializer
	{
		private readonly IEmailResourceProvider _emailResourceProvider;
		private readonly ITemplateService _templateService;

		public RazorEmailTemplateInitializer(IEmailResourceProvider emailResourceProvider, ITemplateService templateService)
		{
			_emailResourceProvider = emailResourceProvider;
			_templateService = templateService;
		}

		public void CompileTemplatesForTypesInSameNamespaceAs<TModel>(string typesEndingWith = "Model")
		{
			// Currently get all types from the same namespace - we may want to do some filtering here or in the compilation step
			var modelTypes = typeof(TModel).Assembly.GetTypes()
				.Where(t => t.Namespace == typeof(TModel).Namespace)
				.Where(t => t.Name.EndsWith(typesEndingWith));
			foreach (var modelType in modelTypes)
			{
				foreach (var languageCode in _emailResourceProvider.GetSupportedLanguages())
				{
					using (new LanguageScope(languageCode))
					{
						CompileLayouts(languageCode);
						CompileTemplatesFor(modelType, languageCode);
					}
				}
			}
		}

		private void CompileLayouts(string languageCode)
		{
			var plainTextLayoutTemplate = _emailResourceProvider.GetPlainTextLayoutTemplate();
			_templateService.Compile(plainTextLayoutTemplate, typeof(string), GetPlainTextLayoutName(languageCode));

			var htmlLayoutTemplate = _emailResourceProvider.GetHtmlLayoutTemplate();
			_templateService.Compile(htmlLayoutTemplate, typeof(string), GetHtmlLayoutName(languageCode));
		}

		private void CompileTemplatesFor(Type modelType, string languageCode)
		{
			var subjectTemplate = _emailResourceProvider.GetSubjectTemplate(modelType);
			_templateService.Compile(subjectTemplate, modelType, GetSubjectTemplateName(modelType, languageCode));

			var plainTextTemplate = _emailResourceProvider.GetPlainTextBodyTemplate(modelType);
			_templateService.Compile(plainTextTemplate, modelType, GetPlainTextTemplateName(modelType, languageCode));

			var htmlTemplate = _emailResourceProvider.GetHtmlBodyTemplate(modelType);
			_templateService.Compile(htmlTemplate, modelType, GetHtmlTemplateName(modelType, languageCode));
		}

		public string GetPlainTextLayoutName(string languageCode)
		{
			return "PlainTextLayout-{0}".FormatWith(languageCode);
		}

		public string GetHtmlLayoutName(string languageCode)
		{
			return "HtmlLayout-{0}".FormatWith(languageCode);
		}

		public string GetSubjectTemplateName(Type modelType, string languageCode)
		{
			return "{0}-Subject-{1}".FormatWith(modelType.FullName, languageCode);
		}

		public string GetPlainTextTemplateName(Type modelType, string languageCode)
		{
			return "{0}-PlainText-{1}".FormatWith(modelType.FullName, languageCode);
		}

		public string GetHtmlTemplateName(Type modelType, string languageCode)
		{
			return "{0}-Html-{1}".FormatWith(modelType.FullName, languageCode);
		}
	}
}