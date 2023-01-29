using System.ComponentModel;
using _GameFiles.Scripts.EventArgs;
using DG.Tweening;
using TadPoleFramework;
using TadPoleFramework.Core;
using UnityEngine;

namespace _GameFiles.Scripts.Managers
{
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
            Debug.Log("Restart Level");
            StopAllCoroutines();
            DOTween.KillAll();
            BroadcastUpward(new SceneStartedEventArgs());
            Broadcast(new SceneStartedEventArgs());
        }
        public void InjectModel(GameModel gameModel)
        {
            _gameModel = gameModel;
            _gameModel.PropertyChanged += GameMOdelProperetyChangedHandler;
        }
        private void GameMOdelProperetyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_gameModel.Level))
            {
                
            }
        }
    }
}