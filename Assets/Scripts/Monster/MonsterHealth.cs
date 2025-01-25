using Managers;
using UnityEngine;

namespace Monster
{
    public class MonsterHealth : MonoBehaviour
    {
        private static readonly int Hits = Animator.StringToHash("Hits");
        [SerializeField] private Animator animator;
        [SerializeField] private float recoverTime;
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
                    animator.SetInteger(Hits, _hits);
                    _recoverTimer = recoverTime;
                }
                else
                {
                    _recoverTimer -= Time.deltaTime;
                }
            }
        }

        private void OnEnable()
        {
            EventManager.HitMonster += AddHit;
        }

        private void OnDisable()
        {
            EventManager.HitMonster -= AddHit;
        }

        private void AddHit(int hit)
        {
            _hits++;
            animator.SetInteger(Hits, _hits);
            _recoverTimer = recoverTime;
            if (_hits == 4)
            {
                Destroy(gameObject,1);
            }
        }
    }
}
