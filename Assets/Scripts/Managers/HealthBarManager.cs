using System;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Managers
{
    public class HealthBarManager : MonoBehaviour
    {
        [SerializeField] private Tilemap healthTilemap;
        [SerializeField] private TileBase blackTile;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void OnEnable()
        {
            EventManager.PlayerGotHit += OneHeart;
            EventManager.GameOver += ZeroHeart;
        }

        private void OnDisable()
        {
            EventManager.PlayerGotHit -= OneHeart;
            EventManager.GameOver -= ZeroHeart;
        }

        private void OneHeart(bool obj)
        {
            PlaceBlackTile(new Vector3Int(-5,-13, 0));
        }
        private void ZeroHeart(bool obj)
        {
            PlaceBlackTile(new Vector3Int(-6,-13, 0));
        }
        
        
        private void PlaceBlackTile(Vector3Int cellPos)
        {
            healthTilemap.SetTile(cellPos, blackTile);
        }
    }
}
