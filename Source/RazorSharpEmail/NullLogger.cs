using System;

namespace RazorSharpEmail
{
    public class NullLogger : ILogger
    {
        static readonly Lazy<ILogger> _instance = new Lazy<ILogger>(() => new NullLogger());
        public static ILogger Instance { get { return _instance.Value; } }

        private NullLogger()
        {
        }

        public void Info(Action message)
        {
            
        }
    }
}