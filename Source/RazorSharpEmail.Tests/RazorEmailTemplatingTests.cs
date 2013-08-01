using System;
using Email.Models;
using NUnit.Framework;

namespace RazorSharpEmail.Tests
{
    [TestFixture]
    public class RazorEmailTemplatingTests
    {
        private IEmailFormatter _emailFormatter;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _emailFormatter = Given.AnInitializedEmailFormatter();
        }

        [Test]
        public void Given_an_email_formatter_when_attempting_to_format_a_message_without_a_language_scope_it_should_throw()
        {
            Assert.Throws<InvalidOperationException>(() => _emailFormatter.BuildTemplatedEmailFrom(new SimpleEmailModel()));
        }

        [Test]
        public void Given_an_email_formatter_when_attempting_to_format_a_message_in_a_language_without_templates_it_should_throw()
        {
            using (new LanguageScope("pl-PL"))
            {
                Assert.Throws<InvalidOperationException>(() => _emailFormatter.BuildTemplatedEmailFrom(new SimpleEmailModel()));
            }
        }
        
        [Test]
        public void Given_an_email_formatter_when_attempting_to_format_a_message_using_the_default_language_it_should_not_throw()
        {
            try
            {
                LanguageScope.SetDefaultLanguage("en-AU");
                _emailFormatter.BuildTemplatedEmailFrom(new SimpleEmailModel());
            }
            finally
            {
                LanguageScope.ClearDefaultLanguage();
            }
        }
    }
}
