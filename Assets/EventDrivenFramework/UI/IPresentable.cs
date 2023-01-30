namespace EventDrivenFramework.UI
{
    public interface IPresentable
    {
        void InjectPresenter(BasePresenter presenter);
    }
}