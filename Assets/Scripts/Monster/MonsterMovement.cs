using System;
using System.Collections;
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
        private bool _gameStarted;
        private bool _hitByRock;

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
            if (_gameStarted)
            {
                transform.position += _currentDirection * (Time.deltaTime * speed);

                if ((IsColliding(_currentDirection) || CloseToBound()) && !_hitByRock)
                {
                    MonsterStateManager.Instance.RollStateDice(this.gameObject);
                    ChooseNewDirection();
                }
            }
        }

        private bool IsColliding(Vector3 direction)
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCollider2D.size, 0f, direction,
                tilemap.cellSize.x / 2, obstacleLayer);

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

            _currentDirection = availableDirections.Count > 0
                ? availableDirections[Random.Range(0, availableDirections.Count)]
                : Vector2.zero;
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
                StartCoroutine(PlayGotHitSound());
                EventManager.PlayerGotHit?.Invoke(true);
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

        private IEnumerator PlayGotHitSound()
        {
            FindAnyObjectByType<SoundManager>().Play("hitWithMonster");
            yield return new WaitForSeconds(FindAnyObjectByType<SoundManager>().getSoundClip("hitWithMonster").length);
            FindAnyObjectByType<SoundManager>().Play("lifeLost");

        }

        private void OnEnable()
        {
            EventManager.FinishGameStart += ChangeStartFlag;
        }

        private void OnDisable()
        {
            EventManager.FinishGameStart -= ChangeStartFlag;
        }

        private void ChangeStartFlag(bool obj)
        {
            _gameStarted = true;
        }

        public void ChangeRockFlag()
        {
            _hitByRock = true;
        }

        private bool CloseToBound()
        {
            Vector3 futurePos = _currentDirection * (Time.deltaTime * speed) + transform.position;
            return futurePos.x > 7.5 || futurePos.x < -5.5 || futurePos.y > 2.5 || futurePos.y < -11.5;
        }
    }
}
