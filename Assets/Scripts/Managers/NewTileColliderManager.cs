using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Managers
{
    
    
    public class NewTileColliderManager : MonoBehaviour
    {

        private enum TileNames
        {
            HalfTile,
            FullTile,
            CornerTile,
            Black,
            HalfFull,
            HalfFullFlip,
            NoTile
        }
        
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Tilemap dugTilemap;
        [SerializeField] private List<GameObject> colliderPrefabs;
        [SerializeField] private TileManager tileManager;
        private readonly Dictionary<Vector3Int, GameObject> _currentColliders = new();
        private bool _didntInitDict = true;

        private void Start()
        {
            //putting colliders on all tilemap
            
        }

        public void SetColliderForCellPosition(Vector3Int position, int colliderIndex,  float angle)
        {
            Vector3 worldPos = tilemap.GetCellCenterWorld(position);
            GameObject colliderObject = Instantiate(colliderPrefabs[colliderIndex], worldPos, Quaternion.Euler(0,0,-angle));
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

        private void OnEnable()
        {
            EventManager.UpdateDugTile += UpdateCollider;
        }

        private void OnDisable()
        {
            EventManager.UpdateDugTile -= UpdateCollider;
        }

        private void UpdateCollider(Vector3Int obj)
        {
            if (_currentColliders.ContainsKey(obj))
            {
                RemoveColliderForCellPosition(obj);
                SetColliderForCellPosition(obj,
                    tileManager.GetActiveTiles()[obj].Item1,
                    tileManager.GetActiveTiles()[obj].Item2);
            }
            else
            {
                SetColliderForCellPosition(obj,
                    tileManager.GetActiveTiles()[obj].Item1,
                    tileManager.GetActiveTiles()[obj].Item2);
            }
        }

        private void Update()
        {
            if (_didntInitDict)
            {
                InitDict();
                _didntInitDict = false;
            }
        }

        private void InitDict()
        {
            BoundsInt bounds = tilemap.cellBounds;
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int cellPosition = new Vector3Int(x, y, 0);

                    if (tilemap.HasTile(cellPosition) && !dugTilemap.HasTile(cellPosition))
                    {
                        SetColliderForCellPosition(cellPosition,6, 0);
                    }
                    else if (dugTilemap.HasTile(cellPosition))
                    {
                        Debug.Log("putting (" +tileManager.GetActiveTiles()[cellPosition].Item1 + "," 
                                  + tileManager.GetActiveTiles()[cellPosition].Item2 + ") in: "
                                  +cellPosition);
                        SetColliderForCellPosition(cellPosition,
                            tileManager.GetActiveTiles()[cellPosition].Item1,
                            tileManager.GetActiveTiles()[cellPosition].Item2);
                    }
                }
            }
        }
    }
}