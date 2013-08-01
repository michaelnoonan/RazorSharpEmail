using System;
using Email.Models;
using FluentAssertions;
using NUnit.Framework;
using RazorEngine.Templating;

namespace RazorSharpEmail.Tests
{
    [TestFixture]
    public class RazorEmailTemplatingInitializationTests
    {
        private RazorEmailFormatter _emailFormatter;
        private RazorEmailTemplateInitializer _emailTemplateInitializer;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var templateService = new TemplateService();
        	var embeddedEmailResourceProvider = new EmbeddedEmailResourceProvider(
        		typeof(SimpleEmailModel).Assembly,
        		"Email.RazorTemplates",
        		"Email.Images",
        		"en-AU");
            _emailTemplateInitializer = new RazorEmailTemplateInitializer(embeddedEmailResourceProvider, templateService);
            _emailFormatter = new RazorEmailFormatter(templateService, _emailTemplateInitializer);
        }

        [Test]
        public void Given_an_email_formatter_after_compiling_templates_for_workorder_reports_model_it_should_be_able_to_format_an_email_using_that_model()
        {
            _emailTemplateInitializer.CompileTemplatesForTypesInSameNamespaceAs<SimpleEmailModel>();

            using (new LanguageScope("en-AU"))
            {
                var email = _emailFormatter.BuildTemplatedEmailFrom(
                    new SimpleEmailModel
                    {
                        RecipientFirstName = "Michael",
                        ReferenceNumber = "REF123456",
                        Message = "Hello World!",
                        Url = "http://google.com"
                    });

                email.Should().NotBeNull();
                email.Subject.Should().NotBeBlank();
                email.PlainTextBody.Should().NotBeBlank();
                email.HtmlBody.Should().NotBeBlank();
            }
        }

        [Test]
        public void Given_an_email_formatter_when_compiling_templates_for_a_model_with_no_templates_it_should_throw()
        {
            Assert.Throws<Exception>(() => _emailTemplateInitializer.CompileTemplatesForTypesInSameNamespaceAs<TestWithNoTemplatesEmailModel>());
        }

        [Test]
        public void Given_an_email_formatter_where_the_templates_havent_been_compiled_when_formatting_an_email_it_should_throw()
        {
            Assert.Throws<InvalidOperationException>(() => _emailFormatter.BuildTemplatedEmailFrom(new SimpleEmailModel()));
        }
    }

    public class TestWithNoTemplatesEmailModel {}
}