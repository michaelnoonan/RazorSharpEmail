namespace RazorSharpEmail
{
	public class TemplatedEmail
	{
		public string Subject { get; set; }
		public string PlainTextBody { get; set; }
		public string HtmlBody { get; set; }
	}
}