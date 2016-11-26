namespace Meceqs.Pipeline
{
    /// <summary>
    /// Provides a way for frameworks to automatically set additional properties on every <see cref="MessageContext"/>
    /// before the pipeline is executed.
    /// </summary>
    /// <remarks>
    /// As an example, in ASP.NET Core an implementation could get the current User
    /// from "IHttpContextAccessor" and pass it to <see cref="MessageContext.User"/>.
    /// </remarks>
    public interface IMessageContextEnricher
    {
        void EnrichMessageContext(MessageContext context);
    }
}