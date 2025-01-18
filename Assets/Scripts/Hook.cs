using UnityEngine;

public class Hook : MonoBehaviour
{
    private Vector3 startPoint;
    private Vector3 targetPoint;
    [SerializeField] private float speed = 5;
    private System.Action onReturnCallback;

    private bool isReturning = false;

    public void Initialize(Vector3 startPoint, Vector3 targetPoint, float speed, System.Action onReturnCallback)
    {
        this.startPoint = startPoint;
        this.targetPoint = targetPoint;
        this.speed = speed;
        this.onReturnCallback = onReturnCallback;

        // Adjust the target to keep the Z-axis consistent
        this.targetPoint.z = startPoint.z;
    }

    void Update()
    {
        if (!isReturning)
        {
            // Move towards the target
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

            // Check if it has reached the target
            if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
            {
                isReturning = true; // Start returning to the player
            }
        }
        else
        {
            // Return to the start point
            transform.position = Vector3.MoveTowards(transform.position, startPoint, speed * Time.deltaTime);

            // Check if it has returned to the player
            if (Vector3.Distance(transform.position, startPoint) < 0.1f)
            {
                onReturnCallback?.Invoke(); // Notify the player that the hook has returned
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster") && !isReturning)
        {
            Debug.Log($"Hook hit: {collision.gameObject.name}");
            isReturning = true; // Start returning once it hits a monster
        }
    }
}