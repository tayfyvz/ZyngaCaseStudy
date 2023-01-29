using _GameFiles.Scripts.Controllers;
using DG.Tweening;
using UnityEngine;

namespace _GameFiles.Scripts.Utilities
{
    public static class ChangePosition
    {
        public static void SwapPosition(params PieceController[] pieces)
        {
            Vector3 firstPos = pieces[0].transform.position;
            Vector3 secondPos = pieces[1].transform.position;
            
            (pieces[0].Coordination, pieces[1].Coordination) = (pieces[1].Coordination, pieces[0].Coordination);
            
            Sequence sequence = DOTween.Sequence();
            
            sequence.Append(pieces[0].transform.DOMove(secondPos, .3f).SetEase(Ease.Linear));
            sequence.Join(pieces[1].transform.DOMove(firstPos, .3f).SetEase(Ease.Linear));
            sequence.OnComplete(() =>
            {
                pieces[0].PieceMoved();
                pieces[1].PieceMoved();
            });
        }
        public static void ReSwapPosition(params PieceController[] pieces)
        {
            Vector3 firstPos = pieces[0].transform.position;
            Vector3 secondPos = pieces[1].transform.position;
            
            (pieces[0].Coordination, pieces[1].Coordination) = (pieces[1].Coordination, pieces[0].Coordination);
            
            pieces[0].transform.DOMove(secondPos, .3f).SetEase(Ease.Linear);
            pieces[1].transform.DOMove(firstPos, .3f).SetEase(Ease.Linear);
            
        }

        public static PieceController[,] MovePiece(int row, int column, int moveColumn, PieceController[,] grid)
        {
            PieceController piece = grid[row, column];
            
            piece.Coordination[0] = row;
            piece.Coordination[1] = moveColumn;
            Vector3 pos = new Vector3(row - 3.5f, moveColumn - 6);
            piece.transform.DOMove(pos, .2f);
            
            grid[row, moveColumn] = piece;
            
            return grid;
        }
    }
}