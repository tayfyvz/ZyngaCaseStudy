using _GameFiles.Scripts.EventArgs;
using _GameFiles.Scripts.ScriptableObjects;
using _GameFiles.Scripts.Views;
using TadPoleFramework.Core;
using TadPoleFramework.UI;
using UnityEngine;

namespace _GameFiles.Scripts.Presenters
{
    public class GameViewPresenter : BasePresenter
    {
        [SerializeField] private TimerData timerData;
        public override void Receive(BaseEventArgs baseEventArgs)
        {
            switch (baseEventArgs)
            {
                case SceneStartedEventArgs sceneStartedEventArgs:
                    (view as GameView).ActivateView(timerData.Duration, sceneStartedEventArgs.HighScore);
                    break;
                case GridCreatedEventArgs gridCreatedEventArgs:
                    (view as GameView).StartTimer();
                    break;
                case PieceExplodedEventArgs pieceExplodedEventArgs:
                    (view as GameView).SetScoreText();
                    break;
            }
            
        }
        
        public void TimeIsFinished()
        {
            BroadcastUpward(new TimeIsFinishedEventArgs());
        }
    }
}