using _GameFiles.Scripts.Controllers;
using _GameFiles.Scripts.EventArgs;
using EventDrivenFramework.Core;
using GG.Infrastructure.Utils.Swipe;
using EventDrivenFramework.Core;
using UnityEngine;

namespace _GameFiles.Scripts.Managers
{
    //Checks if swipe gesture happened.
    public class SwipeManager : BaseManager
    {
        [SerializeField] private SwipeListener swipeListener;

        private PieceController _pieceController;
        
        private int _row;
        private int _column;

        private bool _isPieceSelected;
        public override void Receive(BaseEventArgs baseEventArgs)
        {
            switch (baseEventArgs)
            {
                case GridCreatedEventArgs:
                    swipeListener.OnSwipe.AddListener(OnSwipe);
                    break;
                case PieceSelectedEventArgs pieceSelectedEventArgs:
                    _isPieceSelected = true;
                    _pieceController = pieceSelectedEventArgs.PieceController;
                    _row = pieceSelectedEventArgs.PieceController.Coordination[0];
                    _column = pieceSelectedEventArgs.PieceController.Coordination[1];
                    break;
                case TimeIsFinishedEventArgs:
                    swipeListener.OnSwipe.RemoveListener(OnSwipe);
                    break;
            }
        }

        private void OnSwipe(string swipe)
        {
            if (_isPieceSelected)
            {
                _pieceController.isClicked = false;
                switch (swipe)
                {
                    case DirectionId.ID_LEFT:
                        Broadcast(new SwipedEventArgs(_row - 1, _column));
                        break;
                    case DirectionId.ID_RIGHT:
                        Broadcast(new SwipedEventArgs(_row + 1, _column));
                        break;
                    case DirectionId.ID_UP:
                        Broadcast(new SwipedEventArgs(_row, _column + 1));
                        break;
                    case DirectionId.ID_DOWN:
                        Broadcast(new SwipedEventArgs(_row, _column - 1));
                        break;
                }
            }

            _isPieceSelected = false;
        }
    }
}