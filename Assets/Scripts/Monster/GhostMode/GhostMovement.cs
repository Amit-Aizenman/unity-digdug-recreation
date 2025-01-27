using System;
using Managers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Monster.GhostMode
{
    public class GhostMovement : MonoBehaviour
    {
        [SerializeField] private Tilemap dugTileMap;
        [SerializeField] private GameObject objectToFollow;
        [SerializeField] private float speed = 3f;
        [SerializeField] private float minRangeToGhost = 2f;
        private float _passedRange;
        private Vector3 _initialPos;
        private Vector3 _targetPos;
        private Vector3Int[] _directions =
        {
            Vector3Int.left,  // Left
            Vector3Int.right, // Right
            Vector3Int.up,    // Up
            Vector3Int.down   // Down
        };

        private void OnEnable()
        {
            _passedRange = minRangeToGhost;
        }

        void Update()
        {
            if (_passedRange > 0)
            {
                FollowObject();
            }
            else
            {
                var targetPos = CheckAdjacentCells();
                if (targetPos != Vector3.zero)
                {
                    MoveToCell(targetPos);
                }
                else
                {
                    FollowObject();
                }
            }
        }

        private void FollowObject()
        {
            if (Vector3.Distance(transform.position, objectToFollow.transform.position) < 0.3f)
            {
                MoveToCell(objectToFollow.transform.position);
            }
            else
            {
                var directionToObject = (objectToFollow.transform.position - transform.position).normalized;
                transform.position += directionToObject * (speed * Time.deltaTime);
                UpdatePassedRange();
            }
        }

        private Vector3 CheckAdjacentCells()
        {
            Vector3Int currentCellPos = dugTileMap.WorldToCell(transform.position);
            if (dugTileMap.HasTile(currentCellPos))
            {
                return dugTileMap.GetCellCenterWorld(currentCellPos);
            }
            foreach (var direction in _directions)
            {
                Vector3Int neighborPosition = currentCellPos + direction;

                if (dugTileMap.HasTile(neighborPosition))
                {
                    return dugTileMap.GetCellCenterWorld(neighborPosition);
                }
            }
            return Vector3.zero;
        }

        private void MoveToCell(Vector3 cellPos)
        {
            var directionToCell = (cellPos - transform.position).normalized;
            if (Vector3.Distance(transform.position, cellPos) < 0.1)
            {
                transform.position = cellPos;
                BackToNormal();
            }
            else
            {
                transform.position += directionToCell * (speed * Time.deltaTime);
                UpdatePassedRange();
            }
        }
        
        private void BackToNormal()
        {
            MonsterStateManager.Instance.RollStateDice(this.gameObject, true);
        }
        
        private void UpdatePassedRange()
        {
            _passedRange -= Time.deltaTime * speed;
        }
    }
}
