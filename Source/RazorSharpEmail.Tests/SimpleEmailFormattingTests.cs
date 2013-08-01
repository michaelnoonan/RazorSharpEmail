using ApprovalTests.Reporters;
using Email.Models;
using NUnit.Framework;

namespace RazorSharpEmail.Tests
{
    [TestFixture]
    [UseReporter(typeof(DiffReporter))]
    public class SimpleEmailFormattingTests
    {
        private IEmailFormatter _emailFormatter;
        private LanguageScope _languageScope;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _emailFormatter = Given.AnInitializedEmailFormatter();
            _languageScope = new LanguageScope("en-AU");
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            _languageScope.Dispose();
        }

        [Test]
        public void Given_a_model_the_email_should_format_the_email_correctly()
        {
            var email = _emailFormatter.BuildTemplatedEmailFrom(
                new SimpleEmailModel
                {
                    RecipientFirstName = "Michael",
                    ReferenceNumber = "REF123456",
                    Message = "Hello World!",
                    Url = "http://google.com"
                });

            ApprovalTests.Approvals.Verify(email.Everything());
        }
    }
}