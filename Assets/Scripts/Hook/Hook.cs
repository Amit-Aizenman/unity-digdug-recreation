using UnityEngine;

public class Hook : MonoBehaviour
{
    [SerializeField] private float speed = 10f; // Speed of the hook
    [SerializeField] float lifetime = 3f; // Time before the hook is destroyed
    private Vector3 direction; // Direction the hook travels

    void Start()
    {
        // Destroy the hook after a set time
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized; // Normalize to ensure consistent speed
    }

    void Update()
    {
        // Move the hook forward
        transform.position += direction * (speed * Time.deltaTime);
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

    void OnHookHit(GameObject target)
    {
        // Add custom logic here, e.g., attach the target or deal damage
        Debug.Log("Hook hit target: " + target.name);
    }
}