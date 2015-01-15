using System;
using Castle.Core.Logging;

namespace WebApplication2.Models
{
    public interface IMessageService
    {
        string FormatMessage(String message);
    }

    public class HelloService : IMessageService
    {
        public HelloService()
        {
        }

        public ILogger Logger { get; set; }

        public string FormatMessage(String message)
        {
            return String.Format("Hello {0}", message);
        }
    }

    public class HowdyService : IMessageService
    {
        public HowdyService()
        {
        }
        public string FormatMessage(String message)
        {
            return String.Format("Howdy {0}", message);
        }
    }
}