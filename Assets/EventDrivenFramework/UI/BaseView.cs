using UnityEngine;

namespace EventDrivenFramework.UI
{
    public abstract class BaseView : MonoBehaviour, IPresentable
    {
        protected BasePresenter _presenter;

        protected virtual void Awake()
        {
            gameObject.SetActive(false);
        }

        public void InjectPresenter(BasePresenter presenter)
        {
            _presenter = presenter;
            Initialize();
        }

        protected abstract void Initialize();
    }
}