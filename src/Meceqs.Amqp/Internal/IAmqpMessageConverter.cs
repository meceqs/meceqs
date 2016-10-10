using Amqp;

namespace Meceqs.Amqp.Internal
{
    public interface IAmqpMessageConverter
    {
        Message ConvertToAmqpMessage(Envelope envelope);

        Envelope ConvertToEnvelope(Message amqpMessage);
    }
}