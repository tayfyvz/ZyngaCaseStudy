using System.ComponentModel;
using _GameFiles.Scripts.EventArgs;
using _GameFiles.Scripts.Models;
using DG.Tweening;
using EventDrivenFramework.Core;
using EventDrivenFramework;
using EventDrivenFramework.Core;
using UnityEngine;

namespace _GameFiles.Scripts.Managers
{
    //Triggers the scene start event in the beginning
    // and restarts the level when grid is destroyed.
    public class LevelManager : BaseManager
    {
        private GameModel _gameModel;
        public override void Receive(BaseEventArgs baseEventArgs)
        {
            switch (baseEventArgs)
            {
                case GridDestroyedEventArgs:
                    RestartLevel();
                    break;
            }
        }

        protected override void Start()
        {
            BroadcastUpward(new SceneStartedEventArgs());
            Broadcast(new SceneStartedEventArgs());
        }

        private void RestartLevel()
        {
            StopAllCoroutines();
            DOTween.KillAll();
            BroadcastUpward(new SceneStartedEventArgs());
            Broadcast(new SceneStartedEventArgs());
        }
    }
}