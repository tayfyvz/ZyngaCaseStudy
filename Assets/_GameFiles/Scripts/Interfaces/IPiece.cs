using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _GameFiles.Scripts.Interfaces
{
    public interface IPiece
    {
        public SpriteRenderer SpriteRenderer { get; }
        public int[] Coordination { get; }
        public ColorType PieceColorType { get;}
        public enum ColorType 
        {
            Blue,
            Green,
            Purple,
            Red,
            Yellow
        }

        public void SetPiece(Vector3 pos, Sprite sprite, int[] coordinates, ColorType colorType);
        

    }
}
