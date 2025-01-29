using Managers;
using UnityEngine;

namespace Hook
{
    public class Hook : MonoBehaviour
    {
        [SerializeField] private float speed = 10f; // Speed of the hook
        [SerializeField] float lifetime = 0.5f; // Time before the hook is destroyed
        private bool _isHooked;
        private Vector3 _direction; // Direction the hook travels

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
                    DestoryHookMask();
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
                DestoryHookMask();

            }

            if (other.gameObject.CompareTag("Pooka"))
            {
                _isHooked = true;
                speed = 0;
                EventManager.HitMonster?.Invoke(other.gameObject);
            }
        }

        private void DestroyHook(bool stop)
        {
            if (stop)
            {
                Destroy(gameObject);
                DestoryHookMask();

            }
        }
        
        private void OnEnable()
        {
            EventManager.PlayerStopHitting += DestroyHook;
        }

        private void OnDisable()
        {
            EventManager.PlayerStopHitting -= DestroyHook;
        }

        private void DestoryHookMask()
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag("HookMask");
            foreach (GameObject obj in objects)
            {
                Destroy(obj);
            }
        }

    }
}