using System;
using System.Collections.Generic;
using _GameFiles.Scripts.Controllers;
using DG.Tweening;
using Mono.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

namespace _GameFiles.Scripts.Utilities
{
    public static class PositionUtility
    {
        public static HashSet<PieceController> SwapPositions(PieceController[] pieces, PieceController[,] grid)
        {
            //Checks if its a valid swap
            if (!IsSwappable(pieces, grid))
            {
                //Deactivate the effect
                pieces[0].transform.GetChild(0).gameObject.SetActive(false);
                pieces[0] = pieces[1];
                //Activate the effect
                pieces[0].transform.GetChild(0).gameObject.SetActive(true);
                pieces[1] = null;
                
                return null;
            }

            UpdateGridPositions(pieces, grid);

            HashSet<PieceController> matchSet = new HashSet<PieceController>();

            FindMatches(pieces[0], matchSet, grid);
            FindMatches(pieces[1], matchSet, grid);
            

            
            if (matchSet.Count == 0)
            { 
                UpdateGridPositions(pieces, grid);
                ReturnOriginalPos(pieces);
                return null;
            }
            
            AnimateSwapPos(pieces);
            return matchSet;
        }
        
        //Checks if its a valid swap
        private static bool IsSwappable(PieceController[] pieces, PieceController[,] grid)
        {
            int row1 = pieces[0].Coordination[0];
            int column1 = pieces[0].Coordination[1];

            int row2 = pieces[1].Coordination[0];
            int column2 = pieces[1].Coordination[1];
            
            if ((Math.Abs(row1 - row2) == 1 && Math.Abs(column1 - column2) == 0) || 
                (Math.Abs(row1 - row2) == 0 && Math.Abs(column1 - column2) == 1))
            {
                return true;
            }

            return false;
        }

        //Reswaps invalid position change.
        private static void ReturnOriginalPos(PieceController[] pieces)
        {
            Vector3 originalPos1 = pieces[0].transform.position;
            Vector3 originalPos2 = pieces[1].transform.position;

            Sequence sequence = DOTween.Sequence();
                
            sequence.Append(pieces[0].transform.DOMove(originalPos2, .3f).SetEase(Ease.Linear));
            sequence.Join(pieces[1].transform.DOMove(originalPos1, .3f).SetEase(Ease.Linear));
            sequence.OnComplete(() =>
            {
                Sequence sequence1 = DOTween.Sequence();
                sequence1.Append(pieces[0].transform.DOMove(originalPos1, .3f).SetEase(Ease.Linear));
                sequence1.Join(pieces[1].transform.DOMove(originalPos2, .3f).SetEase(Ease.Linear));
                sequence1.OnComplete((() =>
                {
                    //Deactivate the effect
                    pieces[0].transform.GetChild(0).gameObject.SetActive(false);
                    pieces[0] = null;
                    pieces[1] = null;
                }));
            });
        }

        //Swap animations.
        private static void AnimateSwapPos(PieceController[] pieces)
        {
            Vector3 originalPos1 = pieces[0].transform.position;
            Vector3 originalPos2 = pieces[1].transform.position;

            Sequence sequence = DOTween.Sequence();
                
            sequence.Append(pieces[0].transform.DOMove(originalPos2, .3f).SetEase(Ease.Linear));
            sequence.Join(pieces[1].transform.DOMove(originalPos1, .3f).SetEase(Ease.Linear));
        }
        
        //Reposition the swapped objects.
        private static void UpdateGridPositions(PieceController[] pieces, PieceController[,] grid)
        {
            grid[pieces[0].Coordination[0], pieces[0].Coordination[1]] = pieces[1];
            grid[pieces[1].Coordination[0], pieces[1].Coordination[1]] = pieces[0];
            
            (pieces[0].Coordination, pieces[1].Coordination) = (pieces[1].Coordination, pieces[0].Coordination);
        }
        
        //Finds the matching neighbours of the piece and updates match set. 
        public static void FindMatches(PieceController piece, HashSet<PieceController> matchSet, PieceController[,] grid)
        {
            //Vertical
            int row = piece.Coordination[0];
            int column = piece.Coordination[1];
            HashSet<PieceController> verticalSet = new HashSet<PieceController>();
            if (column > 0)
            {
                for (int i = column - 1; i >= 0; i--)
                {
                    if (grid[row,i].PieceColorType == piece.PieceColorType)
                    {
                        verticalSet.Add(grid[row,i]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (column < grid.GetLength(0) - 1)
            {
                for (int i = column + 1; i < grid.GetLength(0); i++)
                {
                    if (grid[row,i].PieceColorType == piece.PieceColorType)
                    {
                        verticalSet.Add(grid[row,i]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            //Horizontal
            HashSet<PieceController> horizontalSet = new HashSet<PieceController>();
            if (row > 0)
            {
                for (int i = row - 1; i >= 0; i--)
                {
                    if (grid[i,column].PieceColorType == piece.PieceColorType)
                    {
                        horizontalSet.Add(grid[i,column]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (row < grid.GetLength(0) - 1)
            {
                for (int i = row + 1; i < grid.GetLength(0); i++)
                {
                    if (grid[i,column].PieceColorType == piece.PieceColorType)
                    {
                        horizontalSet.Add(grid[i,column]);

                    }
                    else
                    {
                        break;
                    }
                }
            }
            
            if (verticalSet.Count >= 2)
            {
                matchSet.UnionWith(verticalSet);
            }

            if (horizontalSet.Count >= 2)
            {
                matchSet.UnionWith(horizontalSet);
            }

            if (verticalSet.Count >= 2 || horizontalSet.Count >= 2)
            {
                matchSet.Add(piece);
            }
        }
        
        //Moves the falling pieces.
        public static void MovePiece(int row, int column, int moveColumn, PieceController[,] grid)
        {
            PieceController piece = grid[row, column];
            
            piece.Coordination[0] = row;
            piece.Coordination[1] = moveColumn;
            Vector3 pos = new Vector3(row - 3.5f, moveColumn - 6);
            piece.transform.DOMove(pos, .2f);

            grid[row, moveColumn] = piece;
            grid[row, column] = null;
        }
    }
}