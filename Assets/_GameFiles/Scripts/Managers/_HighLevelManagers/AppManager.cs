using EventDrivenFramework.App;
using EventDrivenFramework.Core;
using EventDrivenFramework.Core;
using UnityEngine.Device;

// using ElephantSDK;
// using GameAnalyticsSDK;
namespace _GameFiles.Scripts.Managers._HighLevelManagers
{
    public class AppManager : BaseAppManager
    {
        public override void Receive(BaseEventArgs baseEventArgs)
        {
            
        }

        protected override void Awake()
        {
            base.Awake();
            Screen.SetResolution(800, 1280, true);
        }
        /*protected override void Start()
        {
            base.Start();
            GameAnalytics.Initialize(); 
            LevelStartGA(PlayerPrefs.GetInt("Level"));
        }
        public void LevelCompleteGA(int level)
        {
            GameAnalytics.NewDesignEvent("LevelComplete:Level" + level.ToString("D5"));
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Level " + level.ToString("D5"));
            // Elephant.LevelCompleted(level);
    
        }
        public void LevelStartGA(int level)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "Level " + level.ToString("D5"));
            // Elephant.LevelStarted(level);
        }
        public void LevelFailGA(int level)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "Level " + level.ToString("D5"));
            //Elephant.LevelFailed(level);
        }*/
    }
}