using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _GameFiles.Scripts.Controllers;
using _GameFiles.Scripts.EventArgs;
using _GameFiles.Scripts.Interfaces;
using _GameFiles.Scripts.ScriptableObjects;
using _GameFiles.Scripts.Utilities;
using DG.Tweening;
using TadPoleFramework.Core;
using UnityEngine;

namespace _GameFiles.Scripts.Managers
{
    public class GridManager : BaseManager
    {
        [SerializeField] private GridData gridData;
        
        private readonly HashSet<PieceController> _explodeSet = new HashSet<PieceController>();
        
        private readonly Queue<PieceController> _piecesQueue = new Queue<PieceController>();
        
        private List<Sprite> _sprites = new List<Sprite>();
        private readonly List<PieceController> _movedPieces = new List<PieceController>();
        private readonly List<PieceController> _activePieces = new List<PieceController>();
        
        private PieceController[,] _grid = new PieceController[8, 8];
        private readonly PieceController[] _selectedPieces = new PieceController[2];
        
        private int _movedPieceCounter;

        private bool _isFirstCame;
        private bool _isFirstContinue;
        public override void Receive(BaseEventArgs baseEventArgs)
        {
            switch (baseEventArgs)
            {
                case SceneStartedEventArgs:
                    StartCoroutine(CreateGrid());
                    break;
                case PieceSelectedEventArgs pieceSelectedEventArgs:
                    int row = pieceSelectedEventArgs.Row;
                    int column = pieceSelectedEventArgs.Column;
                    if (row is <= 7 and >= 0 && column is >= 0 and <= 7)
                    {
                        OnPieceControllerSelectedHandler(_grid[row, column]);
                    }
                    break;
                case TimeIsFinishedEventArgs:
                    StopAllCoroutines();
                    DOTween.KillAll();
                    StartCoroutine(DestroyGrid());
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
                    _activePieces.Add(pc);
                    pc.OnPieceControllerSelected += OnPieceControllerSelectedHandler;
                    pc.OnPieceAfterMove += OnPieceAfterMoveHandler;
                    
                    IPiece.ColorType type = GridStartColorCheck.GetPieceType(i, j, _grid);
                    pc.SetPiece(pos,_sprites[(int)type],new[] { i, j }, type);
                    _grid[i, j] = pc;
                    y++;
                    yield return new WaitForSeconds(.001f);
                }
                yield return new WaitForSeconds(.01f);

                x++;
                y = -6f;
            }

            GridCreatedEventArgs gridCreatedEventArgs = new GridCreatedEventArgs();
            Broadcast(gridCreatedEventArgs);
            BroadcastUpward(gridCreatedEventArgs);
            yield return null;
        }

        private IEnumerator DestroyGrid()
        {
            foreach (PieceController piece in _activePieces)
            {
                piece.DestroyPiece();
                _piecesQueue.Enqueue(piece);
                yield return new WaitForSeconds(.001f);
            }

            yield return new WaitForSeconds(1f);
            GridDestroyedEventArgs gridDestroyedEventArgs = new GridDestroyedEventArgs();
            BroadcastUpward(gridDestroyedEventArgs);
            Broadcast(gridDestroyedEventArgs);
        }
        
        private void OnPieceControllerSelectedHandler(PieceController piece)
        {
            if (_selectedPieces[0] == null)
            {
                _movedPieces.Clear();
                _explodeSet.Clear();
                _movedPieceCounter = 0;
                _movedPieces.Add(piece);
                _selectedPieces[0] = piece;
                _selectedPieces[0].transform.GetChild(0).gameObject.SetActive(true);
                Broadcast(new PieceSelectedEventArgs(_selectedPieces[0].Coordination[0], _selectedPieces[0].Coordination[1]));
                piece.RayThrower(true);
            }
            else if(piece.isChangePlace)
            {
                _selectedPieces[0].transform.GetChild(0).gameObject.SetActive(false);

                _movedPieces.Add(piece);
                _selectedPieces[1] = piece;
                
                _grid[_selectedPieces[0].Coordination[0], _selectedPieces[0].Coordination[1]] = _selectedPieces[1];
                _grid[_selectedPieces[1].Coordination[0], _selectedPieces[1].Coordination[1]] = _selectedPieces[0];
                
                _isFirstCame = false;

                ChangePosition.SwapPosition(_selectedPieces[0], _selectedPieces[1]);
                
                _selectedPieces[0].RayThrower(false);
            }
            else
            {
                _selectedPieces[0].transform.GetChild(0).gameObject.SetActive(false);
                _selectedPieces[0].RayThrower(false);
                _selectedPieces[0] = piece;
                _selectedPieces[0].transform.GetChild(0).gameObject.SetActive(true);
                _selectedPieces[0].RayThrower(true);
            }
            
        }
        private void OnPieceAfterMoveHandler(List<PieceController> explodeList)
        {
            _movedPieceCounter++;

            if (explodeList.Count <= 0 && _selectedPieces[0] != null)
            {
                if (!_isFirstCame)
                {
                    _isFirstCame = true;
                    _isFirstContinue = false;
                }
                else if (!_isFirstContinue)
                {

                    ChangePosition.ReSwapPosition(_selectedPieces[0], _selectedPieces[1]);

                    _grid[_selectedPieces[0].Coordination[0], _selectedPieces[0].Coordination[1]] =
                        _selectedPieces[0];
                    _grid[_selectedPieces[1].Coordination[0], _selectedPieces[1].Coordination[1]] =
                        _selectedPieces[1];

                    _selectedPieces[0] = null;
                    _selectedPieces[1] = null;
                    _movedPieces.Clear();
                    _explodeSet.Clear();
                    _movedPieceCounter = 0;
                    _isFirstCame = false;
                    _isFirstContinue = false;
                    return;
                }
                MovementFinished(_explodeSet.ToList());
                return;
            }

            _isFirstCame = true;
            _isFirstContinue = true;
            _explodeSet.UnionWith(explodeList);
            
             MovementFinished(_explodeSet.ToList());
        }

        private void MovementFinished(List<PieceController> explodeList)
        {
            if (_movedPieceCounter != _movedPieces.Count || _explodeSet.Count == 0)
            {
                return;
            }
            
            HashSet<int> explodedColumnSet = new HashSet<int>();
            
            foreach (PieceController piece in explodeList)
            {
                explodedColumnSet.Add(piece.Coordination[0]);
                Explode(piece);
            }
            
            _movedPieces.Clear();
            _explodeSet.Clear();
            _movedPieceCounter = 0;

            foreach (int row in explodedColumnSet)
            {
                RowCheck(row);
            }

            _selectedPieces[0] = null;
            _selectedPieces[1] = null;
            _isFirstContinue = false;
            _isFirstCame = false;

            StartCoroutine(ReCheck());
        }

        private void RowCheck(int row)
        {
            int maxEmptyColumn = 0;
            int minEmptyColumn = 7;

            for (int i = 0; i < 8; i++)
            {
                if (_grid[row, i] == null)
                {
                    if (i < minEmptyColumn)
                    {
                        minEmptyColumn = i;
                    }

                    if (i > maxEmptyColumn)
                    {
                        maxEmptyColumn = i;
                    }
                }
            }

            FallPieces(row, maxEmptyColumn, minEmptyColumn);
            GetNewPieceToFall(row, 7 - (maxEmptyColumn - minEmptyColumn));
        }

        private void Explode(PieceController piece)
        {
            BroadcastUpward(new PieceExplodedEventArgs());
            ExplodeEffectController.Instance.ExplodeEffect(piece);
            piece.Exploded();
            _piecesQueue.Enqueue(piece);
            _activePieces.Remove(piece);
            piece.OnPieceAfterMove -= OnPieceAfterMoveHandler;
            piece.OnPieceControllerSelected -= OnPieceControllerSelectedHandler;
            _grid[piece.Coordination[0], piece.Coordination[1]] = null;
        }
        private void FallPieces(int row, int column, int minEmptyColumn)
        {
            for (int i = column + 1; i < 8; i++)
            {
                _grid = ChangePosition.MovePiece(row, i, minEmptyColumn, _grid);
                _movedPieces.Add(_grid[row, i]);
                minEmptyColumn++;
            }
        }
        private void GetNewPieceToFall(int row, int minEmptyIndex)
        {
            for (int i = minEmptyIndex; i < 8; i++)
            {

                Vector3 pos = new Vector3(row - 3.5f, i - 6);
                PieceController pc = _piecesQueue.Dequeue();
                _activePieces.Add(pc);
                pc.OnPieceAfterMove += OnPieceAfterMoveHandler;
                pc.OnPieceControllerSelected += OnPieceControllerSelectedHandler;
                IPiece.ColorType type = (IPiece.ColorType)UnityEngine.Random.Range(0, 5);
                pc.SetPiece(pos,_sprites[(int)type],new[] { row, i }, type);
                _grid[row, i] = pc;
                _movedPieces.Add(pc);
            }
        }

        private IEnumerator ReCheck()
        {
            yield return new WaitForSeconds(.4f);
            
            foreach (PieceController piece in _movedPieces)
            {
                piece.PieceMoved();
            }
        }
    }
}