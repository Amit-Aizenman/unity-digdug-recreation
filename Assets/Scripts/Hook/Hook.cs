using UnityEngine;

namespace Hook
{
    public class Hook : MonoBehaviour
    {
        [SerializeField] private float speed = 10f; // Speed of the hook
        [SerializeField] float lifetime = 15; // Time before the hook is destroyed
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

        /*private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object is on the target layer
        if (((1 << collision.gameObject.layer) & collisionMask) != 0)
        {
            Debug.Log("Hook hit: " + collision.name);

            // Handle collision logic (e.g., attaching or dealing damage)
            OnHookHit(collision.gameObject);

            // Destroy the hook after hitting something
            Destroy(gameObject);
        }
    }*/

        private void OnHookHit(GameObject target)
        {
            // Add custom logic here, e.g., attach the target or deal damage
            Debug.Log("Hook hit target: " + target.name);
        }
    }
}