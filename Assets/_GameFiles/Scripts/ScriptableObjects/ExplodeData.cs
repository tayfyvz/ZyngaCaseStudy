using UnityEngine;

namespace _GameFiles.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "explodeData", menuName = "Explode Data", order = 54)]
    public class ExplodeData : ScriptableObject
    {
        [SerializeField] private ParticleSystem effect;
        [SerializeField] private int maxEffect;
        
        public ParticleSystem Effect => effect;
        public int MaxEffect => maxEffect;

    }
}