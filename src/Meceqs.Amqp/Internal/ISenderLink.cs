using System;
using System.Threading.Tasks;
using Amqp;

namespace Meceqs.Amqp.Internal
{
    public interface ISenderLink : IDisposable
    {
        Task SendAsync(Message message);
    }
}