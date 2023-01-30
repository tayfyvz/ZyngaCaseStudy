using System;
using _GameFiles.Scripts.Interfaces;
using DG.Tweening;
using UnityEngine;

namespace _GameFiles.Scripts.Controllers
{
    //Jewel object and its controller.
    public class PieceController : MonoBehaviour, IPiece
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private IPiece.ColorType pieceColorType;
        [SerializeField] private int[] coordination = new int[2];
        [SerializeField] private SpriteMask spriteMask;
        
        public Action<PieceController> OnPieceControllerMouseDown { get; set; }
        public Action<PieceController> OnPieceControllerMouseUp { get; set; }
        
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public IPiece.ColorType PieceColorType => pieceColorType;
        public SpriteMask SpriteMask => spriteMask;
        public int[] Coordination
        {
            get => coordination;
            set => coordination = value;
        }
        
        public bool isClicked;
        private bool _isPlaced;
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteMask = transform.GetChild(1).GetComponent<SpriteMask>();
        }
        public void SetPiece(Vector3 pos, Sprite sprite, int[] coordinates, IPiece.ColorType colorType)
        {
            transform.position = new Vector3(pos.x, pos.y + 10, pos.z);
            
            spriteRenderer.sprite = sprite;
            spriteMask.sprite = sprite;
            
            coordination[0] = coordinates[0];
            coordination[1] = coordinates[1];

            pieceColorType = colorType;
            
            gameObject.SetActive(true);

            transform.DOMove(pos, .3f).SetEase(Ease.Linear).OnComplete((() =>
            {
                _isPlaced = true;
            }));
        }

        public void DestroyPiece()
        {
            isClicked = false;
            Vector3 pos = new Vector3(transform.position.x, 10, transform.position.z);
            transform.DOMove(pos, .3f).SetEase(Ease.InBounce).OnComplete((() =>
            {
                gameObject.SetActive(false);
            }));
        }

        public void Exploded()
        {
            _isPlaced = false;
            isClicked = false;
            gameObject.SetActive(false);
        }
        
        private void OnMouseDown()
        {
            if (_isPlaced)
            {
                isClicked = true;
                OnPieceControllerMouseDown?.Invoke(this);
            }
        }

        private void OnMouseUp()
        {

            if (isClicked)
            {
                OnPieceControllerMouseUp?.Invoke(this);
            }
        }
    }
}
