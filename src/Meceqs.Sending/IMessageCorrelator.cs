namespace Meceqs.Sending
{
    public interface IMessageCorrelator
    {
        void CorrelateSourceWithTarget(IMessageEnvelope source, IMessageEnvelope target);
    }
}