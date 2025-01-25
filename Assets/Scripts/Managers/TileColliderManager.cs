using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Managers
{
    public class TileColliderManager : MonoBehaviour
    {

        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Tilemap dugTilemap;
        [SerializeField] private GameObject colliderPrefab;
        private readonly Dictionary<Vector3Int, GameObject> _currentColliders = new();

        private void Start()
        {
            //putting colliders on all tilemap
            BoundsInt bounds = tilemap.cellBounds;
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int cellPosition = new Vector3Int(x, y, 0);

                    if (tilemap.HasTile(cellPosition) && !dugTilemap.HasTile(cellPosition))
                    {
                        SetColliderForCellPosition(cellPosition);
                    }
                }
            }
        }

        public void SetColliderForCellPosition(Vector3Int position)
        {
            Vector3 worldPos = tilemap.GetCellCenterWorld(position);
            GameObject colliderObject = Instantiate(colliderPrefab, worldPos, Quaternion.identity);
            _currentColliders[position] = colliderObject;
        }

        public void RemoveColliderForCellPosition(Vector3Int position)
        {
            if (_currentColliders.TryGetValue(position, out GameObject colliderObject))
            {
                Destroy(colliderObject); // Destroy the collider GameObject
                _currentColliders.Remove(position); // Remove from the dictionary
            }
        }
    }
}
