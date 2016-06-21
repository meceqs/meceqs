namespace Meceqs.Sending
{
    public class DefaultMessageCorrelator : IMessageCorrelator
    {
        public void CorrelateSourceWithTarget(MessageEnvelope source, MessageEnvelope target)
        {
            if (source == null || target == null)
                return;

            target.CorrelationId = source.CorrelationId;
        }
    }
}