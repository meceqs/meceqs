namespace Meceqs.Sending
{
    public interface IEnvelopeCorrelator
    {
        void CorrelateSourceWithTarget(Envelope source, Envelope target);
    }
}
