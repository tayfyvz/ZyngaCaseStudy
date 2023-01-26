using System;
using System.Collections.Generic;
using _GameFiles.Scripts.Controllers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace _GameFiles.Scripts.Interfaces
{
    public interface IPiece
    {
        public Action<PieceController> OnPieceControllerSelected { get; set; }
        public Action<List<PieceController>> OnPieceAfterMove { get; set; }

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

        public void SetPiece(Vector3 pos, Sprite sprite, int[] coordinates, ColorType colorType);
        public void Exploded();


    }
}
