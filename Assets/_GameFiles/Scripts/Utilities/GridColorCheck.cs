using System.Collections.Generic;
using _GameFiles.Scripts.Controllers;
using _GameFiles.Scripts.Interfaces;
using UnityEngine;

namespace _GameFiles.Scripts.Utilities
{
    public static class GridColorCheck 
    {
        public static IPiece.ColorType CheckGrid(int i, int j, PieceController[,] pieces)
        {
            if (i < 2 && j < 2)
            {
                return (IPiece.ColorType)UnityEngine.Random.Range(0, 5);
            }

            List<IPiece.ColorType> takenColors = new List<IPiece.ColorType>();
            
            if (i >= 2)
            {
                if (pieces[i-1,j].PieceColorType == pieces[i - 2,j].PieceColorType)
                {
                    takenColors.Add(pieces[i-1,j].PieceColorType);
                }
            }

            if (j >= 2)
            {
                if (pieces[i,j - 1].PieceColorType == pieces[i,j - 2].PieceColorType)
                {
                    takenColors.Add(pieces[i,j - 1].PieceColorType);
                }
            }

            if (takenColors.Count <= 0) return (IPiece.ColorType)UnityEngine.Random.Range(0, 5);
            IPiece.ColorType type;
            do
            {
                type = (IPiece.ColorType)UnityEngine.Random.Range(0, 5);
            } while (takenColors.Contains(type));
            return type;

        }
    }
}
