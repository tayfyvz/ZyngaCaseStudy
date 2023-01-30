using System.ComponentModel;
using _GameFiles.Scripts.EventArgs;
using _GameFiles.Scripts.Models;
using TadPoleFramework;
using TadPoleFramework.Core;
using TadPoleFramework.Game;
using UnityEngine;

namespace _GameFiles.Scripts.Managers._HighLevelManagers
{
    //Builds the event driven infrastructure by setting up the communication between
    // level manager, grid manager, swipe manager and UI manager.
    public class GameManager : BaseGameManager
    {
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private SwipeManager swipeManager;
        
        private GameModel _gameModel;
        public override void Receive(BaseEventArgs baseEventArgs)
        {
            switch (baseEventArgs)
            {
                case SceneStartedEventArgs sceneStartedEventArgs:
                    Broadcast(sceneStartedEventArgs);
                    _gameModel.InstantScore = 0;
                    break;
                case GridCreatedEventArgs gridCreatedEventArgs:
                    Broadcast(gridCreatedEventArgs);
                    break;
                case TimeIsFinishedEventArgs timeIsFinishedEventArgs:
                    BroadcastDownward(timeIsFinishedEventArgs);
                    if (_gameModel.InstantScore > _gameModel.Score)
                    {
                        _gameModel.Score = _gameModel.InstantScore;
                    }
                    break;
                case GridDestroyedEventArgs gridDestroyedEventArgs:
                    Broadcast(gridDestroyedEventArgs);
                    break;
                case PieceExplodedEventArgs pieceExplodedEventArgs:
                    _gameModel.InstantScore++;
                    Broadcast(pieceExplodedEventArgs);
                    break;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            IMediator mediator = new BaseMediator();
            levelManager.InjectMediator(mediator);
            levelManager.InjectManager(this);
            
            gridManager.InjectMediator(mediator);
            gridManager.InjectManager(this);
            
            swipeManager.InjectMediator(mediator);
            swipeManager.InjectManager(this);
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