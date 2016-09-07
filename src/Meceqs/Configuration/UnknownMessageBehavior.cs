namespace Meceqs.Configuration
{
    /// <summary>
    /// Defines how a filter/transport should treat messages
    /// with an unknown message type.
    /// </summary>
    public enum UnknownMessageBehavior
    {
        ThrowException,

        Skip
    }
}