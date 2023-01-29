using _GameFiles.Scripts.EventArgs;
using GG.Infrastructure.Utils.Swipe;
using TadPoleFramework.Core;
using UnityEngine;

namespace _GameFiles.Scripts.Managers
{
    public class SwipeManager : BaseManager
    {
        [SerializeField] private SwipeListener swipeListener;
        [SerializeField] private bool isPieceSelected;

        [SerializeField] private int row;
        [SerializeField] private int column;

        public override void Receive(BaseEventArgs baseEventArgs)
        {
            switch (baseEventArgs)
            {
                case GridCreatedEventArgs:
                    swipeListener.OnSwipe.AddListener(OnSwipe);
                    break;
                case PieceSelectedEventArgs pieceSelectedEventArgs:
                    isPieceSelected = true;
                    row = pieceSelectedEventArgs.Row;
                    column = pieceSelectedEventArgs.Column;
                    break;
                case TimeIsFinishedEventArgs:
                    swipeListener.OnSwipe.RemoveListener(OnSwipe);
                    break;
            }
        }

        private void OnSwipe(string swipe)
        {
            if (isPieceSelected)
            {
                switch (swipe)
                {
                    case DirectionId.ID_LEFT:
                        Broadcast(new PieceSelectedEventArgs(row - 1, column));
                        break;
                    case DirectionId.ID_RIGHT:
                        Broadcast(new PieceSelectedEventArgs(row + 1, column));
                        break;
                    case DirectionId.ID_UP:
                        Broadcast(new PieceSelectedEventArgs(row, column + 1));
                        break;
                    case DirectionId.ID_DOWN:
                        Broadcast(new PieceSelectedEventArgs(row, column - 1));
                        break;
                }
            }

            isPieceSelected = false;
        }
    }
}