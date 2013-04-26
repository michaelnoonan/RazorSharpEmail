using System;

namespace RazorSharpEmail
{
    public interface IEmailTemplateInitializer
    {
        void CompileTemplatesForTypesInSameNamespaceAs<TModel>(string typesEndingWith = "Model");

        string GetPlainTextLayoutName(string languageCode);

        string GetHtmlLayoutName(string languageCode);

        string GetSubjectTemplateName(Type modelType, string languageCode);

        string GetPlainTextTemplateName(Type modelType, string languageCode);

        string GetHtmlTemplateName(Type modelType, string languageCode);
    }
}