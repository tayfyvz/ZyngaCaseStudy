using System.Collections;
using System.Collections.Generic;
using _GameFiles.Scripts.Controllers;
using _GameFiles.Scripts.Interfaces;
using _GameFiles.Scripts.ScriptableObjects;
using _GameFiles.Scripts.Utilities;
using DG.Tweening;
using TadPoleFramework;
using TadPoleFramework.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _GameFiles.Scripts.Managers
{
    public class GridManager : BaseManager
    {
        [SerializeField] private GridData gridData;
        
        private List<Sprite> _sprites = new List<Sprite>();
        private Queue<PieceController> _piecesQueue = new Queue<PieceController>();
        [SerializeField] private PieceController[] _pieces = new PieceController[2];
        private PieceController[,] _grid = new PieceController[8, 8];
        public override void Receive(BaseEventArgs baseEventArgs)
        {
            switch (baseEventArgs)
            {
                case SceneStartedEventArgs sceneStartedEventArgs:
                    
                    break;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _sprites = new List<Sprite>(Resources.LoadAll<Sprite>("Icons"));
            CreatePiecePool();
        }

        private void CreatePiecePool()
        {
            int maxPiece = gridData.MaxPiece;
            PieceController pcPrefab = gridData.PieceControllerPrefab;

            for (int i = 0; i < maxPiece; i++)
            {
                PieceController pc = Instantiate(pcPrefab, transform);
                pc.gameObject.SetActive(false);
                _piecesQueue.Enqueue(pc);
            }

        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(CreateGrid());
        }

        private IEnumerator CreateGrid()
        {
            float x = -3.5f;
            float y = -6f;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Vector3 pos = new Vector3(x, y, 0);
                    PieceController pc = _piecesQueue.Dequeue();
                    pc.OnPieceControllerSelected += OnPieceControllerSelectedHandler;
                    
                    IPiece.ColorType type = GridColorCheck.CheckGrid(i, j, _grid);
                    pc.SetPiece(pos,_sprites[(int)type],new int[] { i, j }, type);
                    _grid[i, j] = pc;
                    y++;
                    //yield return new WaitForSeconds(.001f);
                }
                //yield return new WaitForSeconds(.01f);

                x++;
                y = -6f;
            }

            yield return null;
        }

        private void OnPieceControllerSelectedHandler(PieceController piece)
        {
            Debug.Log(piece.name + " selected");
            if (_pieces[0] == null)
            {
                _pieces[0] = piece;
                piece.RayThrower(true);
            }
            else if(piece.isChangePlace)
            {
                _pieces[1] = piece;
                SwapPosition(_pieces[0], _pieces[1]);
            }
            else
            {
                _pieces[0].isSelected = false;
                _pieces[0].RayThrower(false);
                _pieces[0] = piece;
                _pieces[0].RayThrower(true);
            }
        }

        private void SwapPosition(params PieceController[] pieces)
        {
            Vector3 firstPos = pieces[0].transform.position;
            Vector3 secondPos = pieces[1].transform.position;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(pieces[0].transform.DOMove(secondPos, .3f).SetEase(Ease.Linear));
            sequence.Join(pieces[1].transform.DOMove(firstPos, .3f).SetEase(Ease.Linear));
            sequence.OnComplete(() =>
            {
                Debug.Log("Swapped");
            });


        }
    }
}