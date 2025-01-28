using Managers;
using UnityEngine;

namespace Player
{
    public class PlayerAttack : MonoBehaviour
    {
        private static readonly int Attack = Animator.StringToHash("attack");

        [SerializeField] private GameObject hookPrefab;
        [SerializeField] private GameObject hookMaskPrefab;
        [SerializeField] private float hookDistanceFromPlayer;
        [SerializeField] private float maskDistanceFromHook;
        [SerializeField] private Animator animator;
        [SerializeField] private const float AttackTime = 1f;

        private GameObject _hook; // To track the spawned hook
        private GameObject _hookMask;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) // When space is pressed
            {
                CastHook();
            }
        }

        void CastHook()
        {
            if (_hook == null) // Ensure only one hook at a time
            {
                // Spawn the hook
                _hook = Instantiate(hookPrefab, GetHookPosition() , Quaternion.identity);
                _hookMask = Instantiate(hookMaskPrefab, GetHookMaskPosition(), Quaternion.identity);
                SetRotation(_hook);
                SetRotation(_hookMask);
                FindAnyObjectByType<SoundManager>().Play("attacking");
            }

            Vector3 direction = GetHookDirection(); // Assuming the player faces right (2D)

            // Pass the direction to the hook
            Hook.Hook hookScript = _hook.GetComponent<Hook.Hook>();
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
                    return transform.position + Vector3.right * hookDistanceFromPlayer;
                case "left":
                    return transform.position + Vector3.left * hookDistanceFromPlayer;
                case "up":
                    return transform.position + Vector3.up * hookDistanceFromPlayer;
                default:
                    return transform.position + Vector3.down * hookDistanceFromPlayer;
            }
        }

        private Vector3 GetHookMaskPosition()
        {
            string dir = PlayerMovement.GetDirection();
            switch (dir)
            {
                case "right":
                    return GetHookPosition() + Vector3.right * maskDistanceFromHook;
                case "left":
                    return GetHookPosition() + Vector3.left * maskDistanceFromHook;
                case "up":
                    return GetHookPosition() + Vector3.up * maskDistanceFromHook;
                default:
                    return GetHookPosition() + Vector3.down * maskDistanceFromHook;
            }
        }

        private Vector3 GetHookDirection()
        {
            string dir = PlayerMovement.GetDirection();
            switch (dir)
            {
                case "right":
                    return transform.right;
                case "left":
                    return -transform.right;
                case "up":
                    return transform.up;
                default:
                    return -transform.up;
            }
        }
        
        private void SetRotation(GameObject obj)
        {
            string dir = PlayerMovement.GetDirection();
            switch (dir)
            {
                case "right":
                    obj.transform.rotation = Quaternion.Euler(0, 0, 0); // No rotation
                    break;
                case "left":
                    obj.transform.rotation = Quaternion.Euler(0, 0, 180); // Rotate 180 degrees
                    break;
                case "up":
                    obj.transform.rotation = Quaternion.Euler(0, 0, 90); // Rotate 90 degrees
                    break;
                case "down":
                    obj.transform.rotation = Quaternion.Euler(0, 0, -90); // Rotate -90 degrees
                    break;
            }
        }

        public static float GetAttackTime()
        {
            return AttackTime;
        }
    }
}