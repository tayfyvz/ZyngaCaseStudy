using _GameFiles.Scripts.Managers._HighLevelManagers;
using _GameFiles.Scripts.Models;
using EventDrivenFramework.Core;
using EventDrivenFramework;
using EventDrivenFramework.Core;
using UnityEngine;

namespace _GameFiles.Scripts.AppInitializer
{
    public class AppInitializer : MonoBehaviour
    {

        [SerializeField] private AppManager appManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private UIManager uiManager;
    
        private void Awake()
        {
            GameModel gameModel = new GameModel();
        
            IMediator mediator = new BaseMediator();
            uiManager.InjectMediator(mediator);
            appManager.InjectMediator(mediator);
            gameManager.InjectMediator(mediator);

            gameManager.InjectModel(gameModel);
            uiManager.InjectModel(gameModel);
        }
    }
}