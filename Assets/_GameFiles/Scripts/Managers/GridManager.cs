using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _GameFiles.Scripts.Controllers;
using _GameFiles.Scripts.Interfaces;
using _GameFiles.Scripts.ScriptableObjects;
using _GameFiles.Scripts.Utilities;
using TadPoleFramework;
using TadPoleFramework.Core;
using UnityEngine;

namespace _GameFiles.Scripts.Managers
{
    public class GridManager : BaseManager
    {
        [SerializeField] private GridData gridData;
        
        private List<Sprite> _sprites = new List<Sprite>();
        private Queue<PieceController> _piecesQueue = new Queue<PieceController>();
        [SerializeField] private PieceController[] _selectedPieces = new PieceController[2];
        
        private PieceController[,] _grid = new PieceController[8, 8];
        [SerializeField] private List<PieceController> _movedPieces = new List<PieceController>();

        private HashSet<PieceController> _explodeSet = new HashSet<PieceController>();
        [SerializeField] private int movedPieceCounter;

        private Coroutine _enumerator;

        public bool _isFirstCame;
        public bool isFirstContinue;
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
                    pc.OnPieceAfterMove += OnPieceAfterMoveHandler;
                    
                    IPiece.ColorType type = GridStartColorCheck.GetPieceType(i, j, _grid);
                    pc.SetPiece(pos,_sprites[(int)type],new int[] { i, j }, type);
                    _grid[i, j] = pc;
                    y++;
                    yield return new WaitForSeconds(.001f);
                }
                yield return new WaitForSeconds(.01f);

                x++;
                y = -6f;
            }

            yield return null;
        }
        
        private void OnPieceControllerSelectedHandler(PieceController piece)
        {
            if (_selectedPieces[0] == null)
            {
                _movedPieces.Clear();
                _explodeSet.Clear();
                movedPieceCounter = 0;
                _movedPieces.Add(piece);
                _selectedPieces[0] = piece;
                _selectedPieces[0].transform.GetChild(0).gameObject.SetActive(true);
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

                _selectedPieces[0].isSelected = false;
                _selectedPieces[1].isSelected = false;

                _selectedPieces[0].RayThrower(false);
            }
            else
            {
                _selectedPieces[0].isSelected = false;
                _selectedPieces[0].transform.GetChild(0).gameObject.SetActive(false);
                _selectedPieces[0].RayThrower(false);
                _selectedPieces[0] = piece;
                _selectedPieces[0].transform.GetChild(0).gameObject.SetActive(true);
                _selectedPieces[0].RayThrower(true);
            }
            
        }
        private void OnPieceAfterMoveHandler(List<PieceController> explodeList)
        {
            movedPieceCounter++;

            if (explodeList.Count <= 0 && _selectedPieces[0] != null)
            {
                if (!_isFirstCame)
                {
                    Debug.Log("VAR");
                    _isFirstCame = true;
                    isFirstContinue = false;
                }
                else if (!isFirstContinue)
                {

                    Debug.Log("RESWAPPOS");
                    ChangePosition.ReSwapPosition(_selectedPieces[0], _selectedPieces[1]);

                    _grid[_selectedPieces[0].Coordination[0], _selectedPieces[0].Coordination[1]] =
                        _selectedPieces[0];
                    _grid[_selectedPieces[1].Coordination[0], _selectedPieces[1].Coordination[1]] =
                        _selectedPieces[1];

                    _selectedPieces[0] = null;
                    _selectedPieces[1] = null;
                    _movedPieces.Clear();
                    _explodeSet.Clear();
                    movedPieceCounter = 0;
                    _isFirstCame = false;
                    isFirstContinue = false;
                    return;
                }
                MovementFinished(_explodeSet.ToList());
                return;
            }

            _isFirstCame = true;
            isFirstContinue = true;
            
            
            _explodeSet.UnionWith(explodeList);
            
             MovementFinished(_explodeSet.ToList());
             
        }

        private void MovementFinished(List<PieceController> explodeList)
        {
            if (movedPieceCounter != _movedPieces.Count || _explodeSet.Count == 0)
            {
                Debug.Log("Time for wait....." + movedPieceCounter + "  total : " + _movedPieces.Count);
                return;
            }
            
            Debug.Log(explodeList.Count + " bu kadar boom");
            
            HashSet<int> explodedColumnSet = new HashSet<int>();
            
            foreach (PieceController piece in explodeList)
            {
                explodedColumnSet.Add(piece.Coordination[0]);
                Explode(piece);
            }

            Debug.Log(explodedColumnSet.Count + " kadar kolon gitti");

            _movedPieces.Clear();
            _explodeSet.Clear();
            movedPieceCounter = 0;

            foreach (int row in explodedColumnSet)
            {
                
                RowCheck(row);
                
            }

            _selectedPieces[0] = null;
            _selectedPieces[1] = null;
            isFirstContinue = false;
            _isFirstCame = false;
            Debug.Log("SIFIRLANDI");

            
            _enumerator = StartCoroutine(ReCheck());
            
            // StartCoroutine(ReCheck());
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

            Debug.Log("Row = " + row + " min Empty index = " + minEmptyColumn + " Max empty index = " + maxEmptyColumn );
            FallPieces(row, maxEmptyColumn, minEmptyColumn);
            GetNewPieceToFall(row, 7 - (maxEmptyColumn - minEmptyColumn));
        }

        private void Explode(PieceController piece)
        {
            piece.Exploded();
            _piecesQueue.Enqueue(piece);
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
                pc.OnPieceAfterMove += OnPieceAfterMoveHandler;
                pc.OnPieceControllerSelected += OnPieceControllerSelectedHandler;
                IPiece.ColorType type = (IPiece.ColorType)UnityEngine.Random.Range(0, 5);
                pc.SetPiece(pos,_sprites[(int)type],new int[] { row, i }, type);
                _grid[row, i] = pc;
                _movedPieces.Add(pc);
            }
        }

        private IEnumerator ReCheck()
        {
            yield return new WaitForSeconds(1.0f);
            
            for (int i = 0; i < _movedPieces.Count; i++)
            {
                Debug.Log("set count : " + _explodeSet.Count);
                _movedPieces[i].PieceMoved();
            }
            // yield return new WaitForSeconds(.5f);
            // _movedPieces.Clear();
            // movedPieceCounter = 0;
            Debug.Log("recheckComplete");
        }
    }
}