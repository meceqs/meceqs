using System;
using Microsoft.Extensions.Options;

namespace Meceqs.Tests
{
    public class TestObjects
    {
        public static MeceqsOptions MeceqsOptions()
        {
            return new MeceqsOptions { ApplicationName = "TestApp", HostName = "TestMachine" };
        }

        public static IEnvelopeFactory EnvelopeFactory()
        {
            return new DefaultEnvelopeFactory(Options.Create(MeceqsOptions()));
        }

        public static Envelope<TMessage> Envelope<TMessage>(TMessage message = null, Guid? id = null)
            where TMessage : class, IMessage, new()
        {
            message = message ?? new TMessage();

            return EnvelopeFactory().Create(message, id ?? Guid.NewGuid());
        }
    }
}