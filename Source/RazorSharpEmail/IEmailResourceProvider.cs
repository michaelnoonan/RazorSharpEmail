using System;
using System.IO;

namespace RazorSharpEmail
{
	public interface IEmailResourceProvider
	{
		string GetTemplate(string name);
	    string GetSubjectTemplate(Type modelType);
	    string GetPlainTextBodyTemplate(Type modelType);
	    string GetHtmlBodyTemplate(Type modelType);
	    string GetPlainTextLayoutTemplate();
	    string GetHtmlLayoutTemplate();
		Stream GetImageStream(string name);
		string[] GetSupportedLanguages();
	}
}