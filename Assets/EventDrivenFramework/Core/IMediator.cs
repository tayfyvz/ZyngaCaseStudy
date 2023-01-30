using EventDrivenFramework.Core;

namespace EventDrivenFramework.Core
{
    public interface IMediator
    {
        void Add(IReceivable receivable);

        void Broadcast(IReceivable receivable, BaseEventArgs baseEventArgs);
    }
}