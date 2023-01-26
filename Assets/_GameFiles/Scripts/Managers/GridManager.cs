using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
        [SerializeField] private PieceController[] _selectedPieces = new PieceController[2];
        
        private PieceController[,] _grid = new PieceController[8, 8];
        private List<PieceController> _movedPieces = new List<PieceController>();
        private bool _isFirstCame;
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
                _selectedPieces[0] = piece;
                piece.RayThrower(true);
            }
            else if(piece.isChangePlace)
            {
                _selectedPieces[1] = piece;

                ChangePosition.SwapPosition(_selectedPieces[0], _selectedPieces[1]);

                int x = _selectedPieces[0].Coordination[0];
                int y = _selectedPieces[0].Coordination[1];
                Debug.Log(_grid[x,y].Coordination[0] + " " +_grid[x,y].Coordination[0] +" once");
                Debug.Log("swapped");
                _grid[x, y] = _selectedPieces[0];
                _grid[_selectedPieces[1].Coordination[0], _selectedPieces[1].Coordination[1]] = _selectedPieces[0];
                Debug.Log(_grid[x,y].Coordination[0] + " " +_grid[x,y].Coordination[0] +" sonra");
                
                _selectedPieces[0].isSelected = false;
                _selectedPieces[1].isSelected = false;

                _selectedPieces[0].RayThrower(false);
            }
            else
            {
                _selectedPieces[0].isSelected = false;
                _selectedPieces[0].RayThrower(false);
                _selectedPieces[0] = piece;
                _selectedPieces[0].RayThrower(true);
            }
        }
        private void OnPieceAfterMoveHandler(List<PieceController> explodeList)
        {
            Debug.Log(explodeList.Count);
            if (explodeList.Count <= 0)
            {
                if (!_isFirstCame)
                {
                    _isFirstCame = true;
                }
                else
                {
                    ChangePosition.ReSwapPosition(_selectedPieces[0], _selectedPieces[1]);
                    
                    _grid[_selectedPieces[0].Coordination[0], _selectedPieces[0].Coordination[1]] = _selectedPieces[1];
                    _grid[_selectedPieces[1].Coordination[0], _selectedPieces[1].Coordination[1]] = _selectedPieces[0];
                    Debug.Log("swapped re");
                    _selectedPieces[0] = null;
                    _selectedPieces[1] = null;
                    _isFirstCame = false;
                }
                return;
            }

            

            HashSet<int> explodedColumnSet = new HashSet<int>();
            
            for (int i = 0; i < explodeList.Count; i++)
            {
                Debug.Log(explodeList[i].PieceColorType, explodeList[i]);
                explodedColumnSet.Add(explodeList[i].Coordination[0]);
                Explode(explodeList[i]);
            }

            if (explodedColumnSet.Count <= 1)
            {
                Debug.Log("Column explode");
                
                List<int> columnList = explodeList.Select(piece => piece.Coordination[1]).ToList();
                columnList.Sort();
                
                if (columnList[^1] < 7)
                {
                    Debug.Log(columnList[^1]);
                    FallPieces(explodedColumnSet.Single(), columnList[^1], columnList[0]);
                }

                //GetNewPieceToFall(explodedColumnSet.Single(), columnList[^1]);
            }
            else
            {
                if (explodedColumnSet.Count != explodeList.Count)
                {
                    Debug.Log("L Shape Explode");
                }
                else
                {
                    Debug.Log("Row Explode");
                    
                }
            }
        }
        
        private void Explode(PieceController piece)
        {
            piece.Exploded();
            _piecesQueue.Enqueue(piece);
        }
        private void GetNewPieceToFall(int row, int column)
        {
            for (int i = column + 1; i < 8; i++)
            {
                Vector3 pos = _grid[row, i].transform.position;
                PieceController pc = _piecesQueue.Dequeue();
                IPiece.ColorType type = (IPiece.ColorType)UnityEngine.Random.Range(0, 5);
                pc.SetPiece(pos,_sprites[(int)type],new int[] { row, i }, type);
                _grid[row, i] = pc;
            }
        }
        private void FallPieces(int row, int column, int minEmptyColumn)
        {
            for (int i = column + 1; i < 8; i++)
            {
                _movedPieces.Add(_grid[row, i]);
                _grid = ChangePosition.MovePiece(row, i, minEmptyColumn, _grid);
                minEmptyColumn++;
            }
        }
    }
}