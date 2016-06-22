namespace Meceqs.Sending
{
    public interface IMessageCorrelator
    {
        void CorrelateSourceWithTarget(Envelope source, Envelope target);
    }
}