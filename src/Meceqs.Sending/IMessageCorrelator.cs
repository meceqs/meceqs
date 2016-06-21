namespace Meceqs.Sending
{
    public interface IMessageCorrelator
    {
        void CorrelateSourceWithTarget(MessageEnvelope source, MessageEnvelope target);
    }
}