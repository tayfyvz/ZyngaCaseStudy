using System.Collections.Generic;
using _GameFiles.Scripts.Interfaces;
using _GameFiles.Scripts.ScriptableObjects;
using _GameFiles.Scripts.Utilities;
using UnityEngine;

namespace _GameFiles.Scripts.Controllers
{
    public class ExplodeEffectController : MonoSingleton<ExplodeEffectController>
    {
        [SerializeField] private ExplodeData explodeData;

        private readonly Queue<ParticleSystem> _effectsQueue = new Queue<ParticleSystem>();
        
        private void Awake()
        {
            int maxPiece = explodeData.MaxEffect;
            ParticleSystem effectPrefab = explodeData.Effect;

            for (int i = 0; i < maxPiece; i++)
            {
                ParticleSystem effect = Instantiate(effectPrefab, transform);
                effect.gameObject.SetActive(false);
                _effectsQueue.Enqueue(effect);
            }
        }

        public void ExplodeEffect(PieceController piece)
        {
            ParticleSystem effect = _effectsQueue.Dequeue();
            effect.transform.position = piece.transform.position;
            effect.gameObject.SetActive(true);
            
            ParticleSystem.MainModule main = effect.main;
            main.startColor = piece.PieceColorType switch
            {
                IPiece.ColorType.Blue => Color.blue,
                IPiece.ColorType.Red => Color.red,
                IPiece.ColorType.Green => Color.green,
                IPiece.ColorType.Purple => Color.magenta,
                IPiece.ColorType.Yellow => Color.yellow,
                _ => main.startColor
            };
            
            effect.Play();
            _effectsQueue.Enqueue(effect);
        }
        
    }
}