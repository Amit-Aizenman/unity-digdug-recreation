using Managers;
using UnityEngine;

namespace Monster
{
    public enum MonsterState
    {
        Running,
        GotHit
    }
    public class MonsterStateMachine : MonoBehaviour
    {
        private static readonly int Hits = Animator.StringToHash("Hits");
        private static readonly int Direction = Animator.StringToHash("Direction");
        [SerializeField] private Animator animator;
        [SerializeField] private MonsterMovement monsterMovement;
        [SerializeField] private MonsterHealth monsterHealth;
        private MonsterState _currentState;
        private float _initialMonsterSpeed;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _currentState = MonsterState.Running;
            _initialMonsterSpeed = monsterMovement.GetSpeed();
        }

        // Update is called once per frame
        void Update()
        {
            switch (_currentState)
            {
                case MonsterState.Running:
                    HandleRunningState();
                    break;
                case MonsterState.GotHit:
                    HandleGotHitState();
                    break;
            }
        }

        private void HandleRunningState()
        {
            if (monsterMovement.GetCurrentDirection() == Vector3.right)
                animator.SetBool(Direction, true);
            else if (monsterMovement.GetCurrentDirection() == Vector3.left)
                animator.SetBool(Direction, false);
        }

        private void HandleGotHitState()
        {
            animator.SetInteger(Hits, monsterHealth.GetHits());
            if (monsterHealth.GetHits() == 0)
            {
                monsterMovement.SetSpeed(_initialMonsterSpeed);
                _currentState = MonsterState.Running;
            }
        }
        
        
        private void OnEnable()
        {
            EventManager.HitMonster += ChangeStateToHit;
        }

        private void OnDisable()
        {
            EventManager.HitMonster -= ChangeStateToHit;
        }
        
        private void ChangeStateToHit(GameObject hitGameObject)
        {
            if (this.gameObject.Equals(hitGameObject))
            {
                monsterHealth.AddHit();
                monsterMovement.SetSpeed(0);
                _currentState = MonsterState.GotHit;
            }
        }
    }
}
