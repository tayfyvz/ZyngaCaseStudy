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
        [SerializeField] private SpriteMask spriteMask;
        
        public Action<PieceController> OnPieceControllerSelected { get; set; }
        public Action<List<PieceController>> OnPieceAfterMove { get; set; }
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public IPiece.ColorType PieceColorType => pieceColorType;
        public SpriteMask SpriteMask => spriteMask;
        public int[] Coordination
        {
            get => coordination;
            set => coordination = value;
        }

        public bool isChangePlace;
        
        private readonly List<float> _angleList = new List<float>
        {
            0,
            90,
            180,
            270
        };
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

            transform.DOMove(pos, .3f).SetEase(Ease.Linear);
        }

        public void DestroyPiece()
        {
            isChangePlace = false;
            Vector3 pos = new Vector3(transform.position.x, 10, transform.position.z);
            transform.DOMove(pos, .3f).SetEase(Ease.InBounce).OnComplete((() =>
            {
                gameObject.SetActive(false);
            }));
        }

        public void Exploded()
        {
            isChangePlace = false;
            gameObject.SetActive(false);
        }

        private void OnMouseDown()
        {
            OnCubeSelected();
        }
        private void OnCubeSelected()
        {
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
