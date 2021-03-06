namespace Meceqs.Transport
{
    /// <summary>
    /// Defines how a middleware/transport should treat messages
    /// with an unknown message type.
    /// </summary>
    public enum UnknownMessageBehavior
    {
        ThrowException,

        Skip
    }
}
