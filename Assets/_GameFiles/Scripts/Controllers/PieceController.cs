using System;
using System.Collections.Generic;
using System.Linq;
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
        public Action<List<PieceController>> OnPieceAfterMove { get; set; }
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public IPiece.ColorType PieceColorType => pieceColorType;
        public int[] Coordination
        {
            get => coordination;
            set => coordination = value;
        }

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

        public void Exploded()
        {
            isSelected = false;
            isChangePlace = false;
            gameObject.SetActive(false);
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

        public void PieceMoved()
        {
            HashSet<PieceController> sameColumnSet = new HashSet<PieceController>();
            HashSet<PieceController> sameRowSet = new HashSet<PieceController>();
            HashSet<PieceController> explodeSet = new HashSet<PieceController>();

            sameColumnSet.UnionWith(RayThrower(0));
            sameColumnSet.UnionWith(RayThrower(180));
            
            sameRowSet.UnionWith(RayThrower(90));
            sameRowSet.UnionWith(RayThrower(270));

            if (sameColumnSet.Count > 2)
            {
                explodeSet.UnionWith(sameColumnSet);
            }

            if (sameRowSet.Count > 2)
            {
                explodeSet.UnionWith(sameRowSet);
            }
            
            OnPieceAfterMove?.Invoke(explodeSet.ToList());
        }
        public void RayThrower(bool condition)
        {
            foreach (float angle in _angleList)
            {
                PieceController cube = CloseDistRaycast(angle);
                
                if (cube != null)
                {
                    cube.isChangePlace = condition;
                }
            }
        }

        private List<PieceController> RayThrower(float angle)
        {
            List<PieceController> connectedPieces = new List<PieceController>();
            PieceController cube = CloseDistRaycast(angle);
                            
            if (cube != null && cube.PieceColorType == pieceColorType)
            {
                connectedPieces.AddRange(cube.RayThrower(angle));
            } 
            connectedPieces.Add(this);
            return connectedPieces;
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
