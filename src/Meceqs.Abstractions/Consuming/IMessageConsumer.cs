namespace Meceqs.Consuming
{
    /// <summary>
    /// This interface is used for sending "an existing envelope from an external caller/system" to a pipeline.
    /// </summary>
    public interface IMessageConsumer
    {
        /// <summary>
        /// <para>Returns a builder object that allows to change the behavior of the consume operation.</para>
        /// <para>If you don't specify a pipeline name, the default "Consume" pipeline will be used.</para>
        /// </summary>
        IFluentConsumer ForEnvelope(Envelope envelope);
    }
}