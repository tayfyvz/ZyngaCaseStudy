using System;
using _GameFiles.Scripts.Controllers;
using UnityEngine;

namespace _GameFiles.Scripts.Interfaces
{
    public interface IPiece
    {
        public Action<PieceController> OnPieceControllerMouseDown { get; set; }
        public Action<PieceController> OnPieceControllerMouseUp { get; set; }

        public SpriteRenderer SpriteRenderer { get; }
        public int[] Coordination { get; set; }
        public ColorType PieceColorType { get;}
        public enum ColorType 
        {
            Blue,
            Green,
            Purple,
            Red,
            Yellow
        }

        public SpriteMask SpriteMask { get;}

        public void SetPiece(Vector3 pos, Sprite sprite, int[] coordinates, ColorType colorType);

        public void DestroyPiece();
        public void Exploded();


    }
}
