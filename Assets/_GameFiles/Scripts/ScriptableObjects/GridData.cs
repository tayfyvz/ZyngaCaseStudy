using _GameFiles.Scripts.Controllers;
using UnityEngine;

namespace _GameFiles.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "gridData", menuName = "Grid Data", order = 52)]
    public class GridData : ScriptableObject
    {
        [SerializeField] private PieceController pieceControllerPrefab;
        [SerializeField] private int maxPiece;
        [SerializeField] private int rowSize;
        [SerializeField] private int columnSize;

        public PieceController PieceControllerPrefab => pieceControllerPrefab;
        public int MaxPiece => maxPiece;
        public int RowSize => rowSize;
        public int ColumnSize => columnSize;

    }
}
