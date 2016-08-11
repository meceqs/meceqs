namespace Meceqs.Tests
{
    public class SimpleMessage : IMessage
    {
        public string SomeKey { get; set; }
    }

    public class SimpleCommand : ICommand
    {
    }

    public class SimpleEvent : IEvent
    {
    }

    public class SimpleResult
    {
        public string Text { get; set; }
    }
}