using EventDrivenFramework.Core;

namespace EventDrivenFramework.Core
{
    public interface IManageable
    {
        void InjectManager(BaseManager manager);
    }
}