namespace Meceqs.Consuming
{
    public interface IMessageConsumer
    {
        IFluentConsumer ForEnvelope(Envelope envelope);
    }
}