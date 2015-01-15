using System;

namespace WebApplication2.Models
{
    public interface IMessageService
    {
        string Message { get; set; }
    }

    public class HelloService : IMessageService
    {
        public HelloService()
        {
            Message = "Hello";
        }
        public string Message { get; set; }

    }

    public class HowdyService : IMessageService
    {
        public HowdyService()
        {
            Message = "Howdy";
        }
        public string Message { get; set; }

    }
}