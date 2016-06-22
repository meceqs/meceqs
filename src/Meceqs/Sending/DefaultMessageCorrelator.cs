namespace Meceqs.Sending
{
    public class DefaultMessageCorrelator : IMessageCorrelator
    {
        public void CorrelateSourceWithTarget(Envelope source, Envelope target)
        {
            if (source == null || target == null)
                return;

            target.CorrelationId = source.CorrelationId;
        }
    }
}