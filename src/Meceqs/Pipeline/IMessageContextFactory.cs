namespace Meceqs.Pipeline
{
    /// <summary>
    /// Creates a <see cref="MessageContext"/> for the given envelope.
    /// </summary>
    public interface IMessageContextFactory
    {
        /// <summary>
        /// Creates a <see cref="MessageContext"/> for the given <paramref name="envelope"/>.
        /// </summary>
        MessageContext CreateMessageContext(Envelope envelope);
    }
}