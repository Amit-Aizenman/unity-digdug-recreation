using System;
using UnityEngine;

namespace Hook
{
    public class Hook : MonoBehaviour
    {
        [SerializeField] private float speed = 10f; // Speed of the hook
        [SerializeField] float lifetime = 0.5f; // Time before the hook is destroyed
        private Vector3 _direction; // Direction the hook travels

        void Start()
        {
            Destroy(gameObject, lifetime); //todo: check why it isn't working
        }

        public void SetDirection(Vector3 newDirection)
        {
            _direction = newDirection.normalized; // Normalize to ensure consistent speed
        }

        void Update()
        {
            // Move the hook forward
            transform.position += _direction * (speed * Time.deltaTime);
        }


        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("SandTile"))
            {
                Destroy(gameObject);
            }
        }
    }
}