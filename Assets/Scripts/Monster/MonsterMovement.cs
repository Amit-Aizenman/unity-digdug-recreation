using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Monster
{
    public class MonsterMovement : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private float speed = 2f;
        [SerializeField] private BoxCollider2D boxCollider2D;
        [SerializeField] private LayerMask obstacleLayer;

        private Vector3 _currentDirection;

        private readonly Vector3[] _straightDirections =
        {
            Vector3.right,
            Vector3.left,
            Vector3.up,
            Vector3.down
        };

        void Start()
        {
            _currentDirection = GetRandomDirection();
        }

        void Update()
        {
            transform.position += _currentDirection * (Time.deltaTime * speed);

            if (IsColliding(_currentDirection))
            {
                ChooseNewDirection();
            }
        }

        private bool IsColliding(Vector3 direction)
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCollider2D.size, 0f, direction, 0.1f, obstacleLayer);

            return hit.collider != null;
        }

        private void ChooseNewDirection()
        {
            List<Vector2> availableDirections = new List<Vector2>();

            foreach (var direction in _straightDirections)
            {
                if (!IsColliding(direction))
                {
                    availableDirections.Add(direction);
                }
            }

            _currentDirection = availableDirections.Count > 0 ?
                availableDirections[Random.Range(0, availableDirections.Count)] : Vector2.zero;
        }

        private Vector3 GetRandomDirection()
        {
            return _straightDirections[Random.Range(0, _straightDirections.Length)];
        }

        void OnDrawGizmos()
        {
            if (boxCollider2D == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + _currentDirection * 0.1f, boxCollider2D.size);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                EventManager.HitPlayer?.Invoke(true);
            }
        }

        public Vector3 GetCurrentDirection()
        {
            return _currentDirection;
        }

        public void SetSpeed(float speed)
        {
            this.speed = speed;
        }

        public float GetSpeed()
        {
            return speed;
        }

        /*private Vector3 FixedPlayerMovement(String wantedDirection)
    {
        var currentCellPos = tilemap.GetCellCenterWorld(tilemap.WorldToCell(transform.position));
        if (currentCellPos == transform.position)
        {
            direction = wantedDirection;
            return _directions[wantedDirection];
        }

        if (wantedDirection.Equals("right") || wantedDirection.Equals("left"))
        {
            if (math.abs(transform.position.y - currentCellPos.y) < 0.1f)
            {
                transform.position = new Vector3(transform.position.x, currentCellPos.y,transform.position.z);
                direction = wantedDirection;
                return _directions[wantedDirection];
            }
            return _directions[direction];
        }
        if (math.abs(transform.position.x - currentCellPos.x) < 0.1f)
        {
            transform.position = new Vector3(currentCellPos.x, transform.position.y,transform.position.z);
            direction = wantedDirection;
            return _directions[wantedDirection];
        }
        return _directions[direction];
    }*/
    }
}
