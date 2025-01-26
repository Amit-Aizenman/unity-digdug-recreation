using Managers;
using UnityEngine;

namespace Monster
{
    public class MonsterHealth : MonoBehaviour
    {
        [SerializeField] private float recoverTime = 1;
        private float _recoverTimer;
        private int _hits;

        private void Start()
        {
            _recoverTimer = recoverTime;
        }
        private void Update()
        {
            if (_hits > 0 && _hits < 4)
            {
                if (_recoverTimer <= 0)
                {
                    _hits--;
                    _recoverTimer = recoverTime;
                }
                else
                {
                    _recoverTimer -= Time.deltaTime;
                }
            }
        }

        public void AddHit()
        {
            _hits++;
            _recoverTimer = recoverTime;
            if (_hits == 4)
            {
                EventManager.MonsterKilled?.Invoke(true);
                Destroy(gameObject,1);
            }
        }

        public int GetHits()
        {
            return _hits;
        }
    }
}
