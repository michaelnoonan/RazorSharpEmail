using System.Runtime.CompilerServices;
using ApprovalTests.Reporters;
using Email.Models;
using NUnit.Framework;
using RazorEngine.Templating;

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
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Given_a_model_the_email_should_format_the_email_correctly()
        {
            var viewBag = new DynamicViewBag(null);
            var email = _emailFormatter.BuildTemplatedEmailFrom(
                new SimpleEmailModel
                {
                    RecipientFirstName = "Michael",
                    ReferenceNumber = "REF123456",
                    Message = "Hello World!",
                    Url = "http://google.com"
                }, viewBag);

            ApprovalTests.Approvals.Verify(email.Everything());
        }

        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Given_a_model_the_email_should_format_and_layout_the_email_correctly()
        {
            var viewBag = new DynamicViewBag(null);
            var templatedEmail = _emailFormatter.BuildTemplatedEmailFrom(
                new SimpleEmailModel
                {
                    RecipientFirstName = "Michael",
                    ReferenceNumber = "REF123456",
                    Message = "Hello World!",
                    Url = "http://google.com"
                }, viewBag);

            ApprovalTests.Approvals.Verify(_emailFormatter.LayoutTemplatedEmail(templatedEmail, viewBag).Everything());
        }
    }
}