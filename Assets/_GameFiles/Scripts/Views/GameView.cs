using System.Collections;
using _GameFiles.Scripts.Presenters;
using DG.Tweening;
using EventDrivenFramework.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _GameFiles.Scripts.Views
{
    // Game view and timer management.
    public class GameView : BaseView
    {
        [SerializeField] private Image background;
        [SerializeField] private Color noTimeColor;
        [SerializeField] private GameObject timer;
        
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI instantScoreText;

        private int _instantScore;
        
        private float _cTime;
        private float _duration;
        private string _minutes;
        private string _seconds;
        protected override void Initialize()
        {
        }

        public void ActivateView(float timerDataDuration, float highScore)
        {
            timer.SetActive(true);
            _instantScore = 0;
            instantScoreText.text = _instantScore + " pts";
            highScoreText.text = "High Score: " + highScore;
            background.color = Color.white;
            timerText.transform.localScale = Vector3.one;
            _duration = timerDataDuration;
            _cTime = timerDataDuration;
            SetTimeText();
            timerText.color = Color.white;
        }
        
        public void StartTimer()
        {
            StartCoroutine(UpdateTime());
        }

        private IEnumerator UpdateTime()
        {
            while (_cTime >= 0)
            {
                if (_cTime <= 15)
                {
                    timerText.color = Color.red;
                    timerText.transform.DOPunchScale(Vector3.one * 0.1f, 1, 1);
                    background.DOColor(noTimeColor, .5f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        background.DOColor(Color.white, .5f);
                    });
                }
                SetTimeText();
                yield return new WaitForSeconds(1);
                _cTime--;
            }

            ResetTimer();
            (_presenter as GameViewPresenter).TimeIsFinished();
            yield return null;
        }

        private void SetTimeText()
        {
            _minutes = ((int)_cTime / 60).ToString();
            _seconds = (_cTime % 60).ToString("00");
            timerText.text = _minutes + ":" + _seconds;
        }
        private void ResetTimer()
        {
            _cTime = _duration;
        }
        public void SetScoreText()
        {
            _instantScore++;
            instantScoreText.text = _instantScore + " pts";
        }
    }
}