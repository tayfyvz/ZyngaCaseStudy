using DG.Tweening;
using EventDrivenFramework.Core;
using EventDrivenFramework.Core;
using UnityEngine;

namespace EventDrivenFramework.App
{
    public abstract class BaseAppManager : BaseManager
    {
        protected virtual void Awake()
        {
            DOTween.SetTweensCapacity(1000, 1000);
            Application.targetFrameRate = 60;
        }
    }
}