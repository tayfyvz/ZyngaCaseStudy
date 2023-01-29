using System.ComponentModel;
using _GameFiles.Scripts.EventArgs;
using _GameFiles.Scripts.Presenters;
using TadPoleFramework;
using TadPoleFramework.Core;
using TadPoleFramework.UI;
using UnityEngine;

namespace _GameFiles.Scripts.Managers._HighLevelManagers
{
    public class UIManager : BaseUIManager
    {
        [SerializeField] private GameViewPresenter gameViewPresenter;
    
        private GameModel _gameModel;
        
        public override void Receive(BaseEventArgs baseEventArgs)
        {
            switch (baseEventArgs)
            {
                case SceneStartedEventArgs:
                    BroadcastDownward(new SceneStartedEventArgs(_gameModel.Score));
                    break;
                case GridCreatedEventArgs gridCreatedEventArgs:
                    BroadcastDownward(gridCreatedEventArgs);
                    break;
                case TimeIsFinishedEventArgs timeIsFinishedEventArgs:
                    Broadcast(timeIsFinishedEventArgs);
                    break;
                case GridDestroyedEventArgs gridDestroyedEventArgs:
                    BroadcastDownward(gridDestroyedEventArgs);
                    break;
                case PieceExplodedEventArgs pieceExplodedEventArgs:
                    BroadcastDownward(pieceExplodedEventArgs);
                    break;
            }
        }
        protected override void Awake()
        {
            base.Awake();
            
            gameViewPresenter.InjectManager(this);
        }

        protected override void Start()
        {
            base.Start();
            gameViewPresenter.ShowView();
        }

        
        public void InjectModel(GameModel gameModel)
        {
            this._gameModel = gameModel;
            this._gameModel.PropertyChanged += GameMOdelProperetyChangedHandler;
        }

        private void GameMOdelProperetyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_gameModel.InstantScore))
            {
            
            }
        }
    }
}