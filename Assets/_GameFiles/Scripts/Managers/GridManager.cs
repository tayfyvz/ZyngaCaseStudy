using System.Collections;
using System.Collections.Generic;
using _GameFiles.Scripts.Controllers;
using _GameFiles.Scripts.Interfaces;
using _GameFiles.Scripts.ScriptableObjects;
using DG.Tweening;
using TadPoleFramework;
using TadPoleFramework.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _GameFiles.Scripts.Managers
{
    public class GridManager : BaseManager
    {
        [SerializeField] private GridData gridData;
        
        private List<Sprite> _sprites = new List<Sprite>();
        private Queue<PieceController> _piecesQueue = new Queue<PieceController>();
        public override void Receive(BaseEventArgs baseEventArgs)
        {
            switch (baseEventArgs)
            {
                case SceneStartedEventArgs sceneStartedEventArgs:
                    
                    break;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _sprites = new List<Sprite>(Resources.LoadAll<Sprite>("Icons"));
            CreatePiecePool();
        }

        private void CreatePiecePool()
        {
            int maxPiece = gridData.MaxPiece;
            PieceController pcPrefab = gridData.PieceControllerPrefab;

            for (int i = 0; i < maxPiece; i++)
            {
                PieceController pc = Instantiate(pcPrefab, transform);
                pc.gameObject.SetActive(false);
                _piecesQueue.Enqueue(pc);
            }

        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(CreateGrid());
        }

        private IEnumerator CreateGrid()
        {
            float x = -3.5f;
            float y = -6f;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Vector3 pos = new Vector3(x, y, 0);
                    PieceController pc = _piecesQueue.Dequeue();
                    int k = UnityEngine.Random.Range(0, 5);
                    pc.SetPiece(pos,_sprites[k],new int[] { i, j }, (IPiece.ColorType)k);
                    y++;
                    //yield return new WaitForSeconds(.001f);
                }
                //yield return new WaitForSeconds(.01f);

                x++;
                y = -6f;
            }

            yield return null;
        }
    }
}