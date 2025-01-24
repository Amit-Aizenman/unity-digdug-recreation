using UnityEngine;

namespace Player
{
    public class PlayerAttack : MonoBehaviour
    {
        private static readonly int Attack = Animator.StringToHash("attack");

        [SerializeField] private GameObject hookPrefab; 
        [SerializeField] private float distanceFromPlayer;
        [SerializeField] private Animator animator;

        private GameObject hook; // To track the spawned hook

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) // When space is pressed
            {
                CastHook();
            }
        }

        void CastHook()
        {
            if (hook == null) // Ensure only one hook at a time
            {
                // Spawn the hook
                hook = Instantiate(hookPrefab, GetHookPosition() , Quaternion.identity);
                animator.SetTrigger(Attack);
            }
            Vector3 direction = transform.right; // Assuming the player faces right (2D)

            // Pass the direction to the hook
            Hook hookScript = hook.GetComponent<Hook>();
            if (hookScript != null)
            {
                hookScript.SetDirection(direction);
            }
        }

        private Vector3 GetHookPosition()
        {
            string dir = PlayerMovement.GetDirection();
            switch (dir)
            {
                case "right":
                    return transform.position + Vector3.right * distanceFromPlayer;
                case "left":
                    return transform.position + Vector3.left * distanceFromPlayer;
                case "up":
                    return transform.position + Vector3.up * distanceFromPlayer;
                default:
                    return transform.position + Vector3.down * distanceFromPlayer;
            }
        }
    }
}