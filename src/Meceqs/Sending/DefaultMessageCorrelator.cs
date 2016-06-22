namespace Meceqs.Sending
{
    public class DefaultMessageCorrelator : IMessageCorrelator
    {
        public void CorrelateSourceWithTarget(IMessageEnvelope source, IMessageEnvelope target)
        {
            if (source == null || target == null)
                return;

            target.CorrelationId = source.CorrelationId;
        }
    }
}