namespace Meceqs.Sending
{
    public class DefaultEnvelopeCorrelator : IEnvelopeCorrelator
    {
        public void CorrelateSourceWithTarget(Envelope source, Envelope target)
        {
            if (source == null || target == null)
                return;

            target.CorrelationId = source.CorrelationId;
        }
    }
}