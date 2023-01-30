using EventDrivenFramework.Core;

namespace EventDrivenFramework.Core
{
    public interface IReceivable
    {
        void Receive(BaseEventArgs baseEventArgs);
    }
}