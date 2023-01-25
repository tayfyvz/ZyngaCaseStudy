using System;
using System.Collections.Generic;
using _GameFiles.Scripts.Interfaces;
using DG.Tweening;
using UnityEngine;

namespace _GameFiles.Scripts.Controllers
{
    public class PieceController : MonoBehaviour, IPiece
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private IPiece.ColorType pieceColorType;
        [SerializeField] private int[] coordination = new int[2];

        public Action<PieceController> OnPieceControllerSelected { get; set; }
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public IPiece.ColorType PieceColorType => pieceColorType;
        public int[] Coordination => coordination;
        public bool isSelected, isChangePlace;
        
        private List<float> _angleList = new List<float>
        {
            0,
            90,
            180,
            270
        };
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        public void SetPiece(Vector3 pos, Sprite sprite, int[] coordinates, IPiece.ColorType colorType)
        {
            transform.position = new Vector3(pos.x, pos.y + 10, pos.z);
            spriteRenderer.sprite = sprite;
            
            coordination[0] = coordinates[0];
            coordination[1] = coordinates[1];

            pieceColorType = colorType;
            
            gameObject.SetActive(true);
            
            transform.DOMove(pos, .3f).SetEase(Ease.Linear);
        }
        
        private void OnMouseDown()
        {
            OnCubeSelected();
        }
        private void OnCubeSelected()
        {
            isSelected = true;
            OnPieceControllerSelected?.Invoke(this);
        }
        
        public void RayThrower(bool condition)
        {
            foreach (float angle in _angleList)
            {
                PieceController cube = CloseDistRaycast(angle);
                
                if (cube != null)
                {
                    Debug.Log(cube.pieceColorType);
                    cube.isChangePlace = condition;
                }
            }
        }
        private PieceController CloseDistRaycast(float angle)
        {
            var direction = Quaternion.AngleAxis(angle, transform.forward) * transform.up;
            
            Debug.DrawRay(transform.position, direction, Color.red, 5);
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 2f, 1 << 6);
            
            return hit ? hit.transform.GetComponent<PieceController>() : null;
        }
    }
}
