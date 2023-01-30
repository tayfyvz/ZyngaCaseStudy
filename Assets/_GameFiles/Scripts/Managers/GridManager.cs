using System.Collections;
using System.Collections.Generic;
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
    /*
     * Inherits BaseManager which gives capability of event broadcasting
     * between managers.
     * 
     * Grid Manager manages the following functionalities;
     * Creation of the grid,
     * Destruction of the grid,
     * Object creation by using object pooling technique,
     * Manages the movement of the jewel objects on the grid.
     * 
     */
    public class GridManager : BaseManager
    {
        [SerializeField] private GridData gridData;
        
        private PieceController[,] _grid;
        
        private Queue<PieceController> _piecesQueue = new Queue<PieceController>();
        
        private List<Sprite> _sprites = new List<Sprite>();
        
        private List<PieceController> _activePieces = new List<PieceController>();
        private List<PieceController> _movedPieces = new List<PieceController>();

        
        private PieceController[] _selectedPieces = new PieceController[2];

        private PieceController _lastClickedPiece;

        private bool _isSwiped;
        
        //Receives triggered events.
        public override void Receive(BaseEventArgs baseEventArgs)
        {
            switch (baseEventArgs)
            {
                case SceneStartedEventArgs:
                    _selectedPieces[0] = null;
                    _selectedPieces[1] = null;
                    StartCoroutine(CreateGrid());
                    break;
                case SwipedEventArgs swipedEventArgs:
                    _selectedPieces[0].transform.GetChild(0).gameObject.SetActive(false);
                    _selectedPieces[0] = _lastClickedPiece;
                    _selectedPieces[0].transform.GetChild(0).gameObject.SetActive(true);
                    _isSwiped = true;
                    if (swipedEventArgs.Row <= _grid.GetLength(0) - 1 && swipedEventArgs.Row >= 0 && 
                        swipedEventArgs.Column <= _grid.GetLength(0) - 1 && swipedEventArgs.Column >= 0)
                    {
                        _selectedPieces[1] = _grid[swipedEventArgs.Row, swipedEventArgs.Column];
                        OnSwipeComplete();
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
            
            //Loads the icon files from resources.
            _sprites = new List<Sprite>(Resources.LoadAll<Sprite>("Icons"));
            
            //Initialize the jewel objects pool.
            //This pool is being used for object pooling methodology.
            CreatePiecePool();
        }

        //Fills the object pooling queue.
        private void CreatePiecePool()
        {
            //Gets the number of jewel objects from configuration container object.
            int maxPiece = gridData.MaxPiece;
            PieceController pcPrefab = gridData.PieceControllerPrefab;

            for (int i = 0; i < maxPiece; i++)
            {
                PieceController pc = Instantiate(pcPrefab, transform);
                pc.gameObject.SetActive(false);
                _piecesQueue.Enqueue(pc);
            }
        }
        
        //Creates grid on game start and time out.
        private IEnumerator CreateGrid()
        {
            _grid = new PieceController[gridData.RowSize, gridData.ColumnSize];
            
            float x = -3.5f;
            float y = -6f;

            for (int i = 0; i < gridData.RowSize; i++)
            {
                for (int j = 0; j < gridData.ColumnSize; j++)
                {
                    Vector3 pos = new Vector3(x, y, 0);
                    PieceController pc = _piecesQueue.Dequeue();
                    _activePieces.Add(pc);

                    //Get the suitable piece type according to its neighbours.
                    //Makes sure three consecutive neighbours are not the same type.
                    IPiece.ColorType type = GridStartColorCheck.GetPieceType(i, j, _grid);
                    
                    //Sets jewel objects icon and position on the grid.
                    pc.SetPiece(pos,_sprites[(int)type],new[] { i, j }, type);
                    
                    _grid[i, j] = pc;
                    y++;
                    
                    //Waits for drop motion.
                    yield return new WaitForSeconds(.001f);
                }
                yield return new WaitForSeconds(.01f);

                x++;
                y = -6f;
            }

            //Adds mouse events after grid creation.
            foreach (PieceController piece in _activePieces)
            {
                piece.OnPieceControllerMouseDown += OnPieceControllerMouseDown;
                piece.OnPieceControllerMouseUp += OnPieceControllerMouseUp;
            }
            
            //Grid creation event is raised.
            //This event triggers SwipeManager and GameManager.
            GridCreatedEventArgs gridCreatedEventArgs = new GridCreatedEventArgs();
            Broadcast(gridCreatedEventArgs);
            BroadcastUpward(gridCreatedEventArgs);
            yield return null;
        }

        //Destroys the grid when the time is up.
        private IEnumerator DestroyGrid()
        {
            _selectedPieces[0] = null;
            _selectedPieces[1] = null;
            
            foreach (PieceController piece in _activePieces)
            {
                piece.DestroyPiece();
                piece.OnPieceControllerMouseDown -= OnPieceControllerMouseDown;
                piece.OnPieceControllerMouseUp -= OnPieceControllerMouseUp;
                _piecesQueue.Enqueue(piece);
            }
            
            _movedPieces.Clear();
            _activePieces.Clear();
            
            yield return new WaitForSeconds(1f);
            
            //Grid destroyed event is raised.
            //This event triggers LevelManager and GameManager.
            GridDestroyedEventArgs gridDestroyedEventArgs = new GridDestroyedEventArgs();
            BroadcastUpward(gridDestroyedEventArgs);
            Broadcast(gridDestroyedEventArgs);
        }
        
        //Triggered when jewel object is tapped.
        private void OnPieceControllerMouseDown(PieceController piece)
        {
            _isSwiped = false;
            //If the selected piece is the first one.
            if (_selectedPieces[0] == null)
            {
                _selectedPieces[0] = piece;
                //Activate the effect
                _selectedPieces[0].transform.GetChild(0).gameObject.SetActive(true);
            }

            _lastClickedPiece = piece;
            Broadcast(new PieceSelectedEventArgs(piece));
        }

        //Triggered when jewel object is selected if not swiped.
        private void OnPieceControllerMouseUp(PieceController piece)
        {

            if (!_isSwiped && _selectedPieces[1] == null && _selectedPieces[0] != piece)
            {
                _selectedPieces[1] = piece;

                //Swaps the position of the jewel objects and initialize rule checking,
                // and gets set of objects to explode.
                HashSet<PieceController> explodeSet = PositionUtility.SwapPositions(_selectedPieces, _grid);

                
                if (explodeSet != null)
                { 
                    //Deactivate the effect
                    _selectedPieces[0].transform.GetChild(0).gameObject.SetActive(false);
                    _selectedPieces[0] = null;
                    _selectedPieces[1] = null;
                    StartCoroutine(ExplodePieces(explodeSet));
                }
            }
        }

        //Triggered when jewel object is swiped.
        private void OnSwipeComplete()
        {
            //Swaps the position of the jewel objects and initialize rule checking,
            // and gets set of objects to explode.
            HashSet<PieceController> explodeSet = PositionUtility.SwapPositions(_selectedPieces, _grid);

            if (explodeSet != null)
            { 
                //Deactivate the effect
                _selectedPieces[0].transform.GetChild(0).gameObject.SetActive(false);
                _selectedPieces[0] = null;
                _selectedPieces[1] = null;
                StartCoroutine(ExplodePieces(explodeSet));
            }
        }
        
        //Explodes the objects, brings down the new jewel objects
        // and triggers object checking recursively for moved objects.
        private IEnumerator ExplodePieces(HashSet<PieceController> explodeSet)
        {
            yield return new WaitForSeconds(.3f);
            
            HashSet<int> rowSet = new HashSet<int>();

            foreach (PieceController piece in explodeSet)
            {
                Explode(piece);
                rowSet.Add(piece.Coordination[0]);
            }
            
            yield return new WaitForSeconds(.3f);
            
            _movedPieces.Clear();
            
            //Brings down new jewel objects
            FallPieces(rowSet);

            //Triggers object checking recursively for moved objects.
            StartCoroutine(CheckMovedObjectRecursively());
            
            yield return null;
        }
        
        //Explodes the jewel, triggers the animation.
        private void Explode(PieceController piece)
        {
            //Animation control. [Singleton]
            ExplodeEffectController.Instance.ExplodeEffect(piece);
            
            piece.Exploded();
            
            //Object pooling queue management.
            _piecesQueue.Enqueue(piece);
            _activePieces.Remove(piece);
            
            piece.OnPieceControllerMouseDown -= OnPieceControllerMouseDown;
            piece.OnPieceControllerMouseUp -= OnPieceControllerMouseUp;
            
            _grid[piece.Coordination[0], piece.Coordination[1]] = null;
            
            BroadcastUpward(new PieceExplodedEventArgs());
        }
        
        //Brings down new jewel objects to fill the empty slots in the grid.
        private void FallPieces(HashSet<int> rowSet)
        {
            foreach (int row in rowSet)
            {
                for (int i = 0; i < _grid.GetLength(1); i++)
                {
                    if (_grid[row, i] == null)
                    {
                        bool flag = false;
                        int emptyPointer = i;
                        
                        for (int j = i + 1; j < _grid.GetLength(0); j++)
                        {
                            if (_grid[row, j] != null)
                            {
                                PositionUtility.MovePiece(row, j, emptyPointer, _grid);
                                _movedPieces.Add(_grid[row, emptyPointer]);
                                flag = true;
                                break;
                            }
                        }

                        if (!flag)
                        {
                            GetNewPieceToFall(row, i);
                        }
                    }
                }
            }
        }
        
        //Polls a new object from object pooling queue and place it on grid.
        private void GetNewPieceToFall(int row, int minEmptyIndex)
        {
            for (int i = minEmptyIndex; i < gridData.ColumnSize; i++)
            {
                PieceController pc = _piecesQueue.Dequeue();
                
                _activePieces.Add(pc);
                pc.OnPieceControllerMouseDown += OnPieceControllerMouseDown;
                pc.OnPieceControllerMouseUp += OnPieceControllerMouseUp;
                
                IPiece.ColorType type = (IPiece.ColorType)UnityEngine.Random.Range(0, 5);
                Vector3 pos = new Vector3(row - 3.5f, i - 6);
                pc.SetPiece(pos,_sprites[(int)type],new[] { row, i }, type);
                
                _grid[row, i] = pc;
                _movedPieces.Add(pc);
            }
        }

        //Checks the moved objects either by user or explosions,
        // and triggers rule checking recursively
        private IEnumerator CheckMovedObjectRecursively()
        {
            HashSet<PieceController> explodeSet = new HashSet<PieceController>();
            
            foreach (PieceController piece in _movedPieces)
            {
                PositionUtility.FindMatches(piece, explodeSet, _grid);
            }
            
            if (explodeSet.Count > 0)
            {
                yield return new WaitForSeconds(.4f);
                StartCoroutine(ExplodePieces(explodeSet));
            }
            else
            {
                _movedPieces.Clear();
                yield return null;
            }
        }
        
    }
}