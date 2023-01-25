using System.ComponentModel;
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

        public override void Receive(BaseEventArgs baseEventArgs)
        {
            switch (baseEventArgs)
            {
            
            }
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