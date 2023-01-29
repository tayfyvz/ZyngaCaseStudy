using UnityEngine;

namespace _GameFiles.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "timerData", menuName = "Timer Data", order = 53)]
    public class TimerData : ScriptableObject
    {
        [SerializeField] private float duration;

        public float Duration => duration;
    }
}