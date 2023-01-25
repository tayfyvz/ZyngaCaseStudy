using System.ComponentModel;
using TadPoleFramework;
using TadPoleFramework.Core;
using TadPoleFramework.Game;
using UnityEngine;

namespace _GameFiles.Scripts.Managers._HighLevelManagers
{
    public class GameManager : BaseGameManager
    {
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private GridManager gridManager;
        
        private GameModel _gameModel;
        public override void Receive(BaseEventArgs baseEventArgs)
        {
            switch (baseEventArgs)
            {
                
            }
        }

        protected override void Awake()
        {
            base.Awake();
            IMediator mediator = new BaseMediator();
            levelManager.InjectMediator(mediator);
            levelManager.InjectManager(this);
            levelManager.InjectModel(_gameModel);
            
            gridManager.InjectMediator(mediator);
            gridManager.InjectManager(this);
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