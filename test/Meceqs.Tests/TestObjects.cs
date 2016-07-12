using System;

namespace Meceqs.Tests
{
    public class TestObjects
    {
        public static ApplicationInfo ApplicationInfo()
        {
            return new ApplicationInfo { ApplicationName = "TestApp", HostName = "TestMachine" };
        }

        public static IEnvelopeFactory EnvelopeFactory()
        {
            return new DefaultEnvelopeFactory(ApplicationInfo());
        }

        public static Envelope<TMessage> Envelope<TMessage>(TMessage message = null, Guid? id = null)
            where TMessage : class, IMessage, new()
        {
            message = message ?? new TMessage();

            return EnvelopeFactory().Create(message, id ?? Guid.NewGuid());
        }
    }
}