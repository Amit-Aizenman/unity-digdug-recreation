using System;
using Managers;
using UnityEngine;

namespace Hook
{
    public class Hook : MonoBehaviour
    {
        [SerializeField] private float speed = 10f; // Speed of the hook
        [SerializeField] float lifetime = 0.5f; // Time before the hook is destroyed
        private bool _isHooked = false;
        private Vector3 _direction; // Direction the hook travels

        void Start()
        {
            Destroy(gameObject, lifetime);
        }

        public void SetDirection(Vector3 newDirection)
        {
            _direction = newDirection.normalized; // Normalize to ensure consistent speed
        }

        void Update()
        {
            if (!_isHooked)
            {
                if (lifetime <= 0)
                {
                    Destroy(gameObject);
                }
                else
                {
                    lifetime -= Time.deltaTime;
                    transform.position += _direction * (speed * Time.deltaTime);
                }
            }
        }


        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("SandTile"))
            {
                Destroy(gameObject);
            }

            if (other.gameObject.CompareTag("Monster"))
            {
                Debug.Log("hit a monster");
                EventManager.HitMonster?.Invoke(1);
            }
        }
        
    }
}