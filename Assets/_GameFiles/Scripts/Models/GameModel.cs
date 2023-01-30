using EventDrivenFramework.Game;
using UnityEngine;

namespace _GameFiles.Scripts.Models
{
    //Contains playerPrefs.
    public class GameModel : BaseGameModel
    {
        #region Score //////////////////////////////////////////////////////
        protected string prefKey_Score = "Score";
        public int Score
        {
            get { return PlayerPrefs.GetInt(prefKey_Score); }
            set
            {
                PlayerPrefs.SetInt(prefKey_Score, value);
                OnPropertyChanged(nameof(Score));
            }
        }
        #endregion
        
        #region InstantScore //////////////////////////////////////////////////////
        protected string prefKey_InstantScore = "InstantScore";
        public int InstantScore
        {
            get { return PlayerPrefs.GetInt(prefKey_InstantScore); }
            set
            {
                PlayerPrefs.SetInt(prefKey_InstantScore, value);
                OnPropertyChanged(nameof(InstantScore));
            }
        }
        #endregion
        
        
        public GameModel()
        {
            #region Score //////////////////////////////////////////////////
            if (!PlayerPrefs.HasKey(prefKey_Score))
                Score = 0;
            else
                Score = PlayerPrefs.GetInt(prefKey_Score);
            #endregion
            
            #region InstantScore //////////////////////////////////////////////////
            if (!PlayerPrefs.HasKey(prefKey_InstantScore))
                InstantScore = 0;
            else
                InstantScore = PlayerPrefs.GetInt(prefKey_InstantScore);
            #endregion
            
        }
    }
}