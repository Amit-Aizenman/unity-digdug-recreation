using Managers;
using UnityEngine;

namespace Monster
{
    public enum MonsterState
    {   
        Starting,
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
        private float _initialAnimationSpeed;
        private Vector3 _initialMonsterPosition;
        private bool _isHooked;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            _currentState = MonsterState.Starting;
            _initialMonsterSpeed = monsterMovement.GetSpeed();
            _initialMonsterPosition = transform.position;
            _initialAnimationSpeed = animator.speed;
        }

        // Update is called once per frame
        private void Update()
        {
            switch (_currentState)
            {
                case MonsterState.Running:
                    HandleRunningState();
                    break;
                case MonsterState.GotHit:
                    HandleGotHitState();
                    break;
                case MonsterState.Starting:
                    HandleStartingState();
                    break;
            }
        }

        private void HandleStartingState()
        {
            animator.speed = 0;
            Debug.Log("monster animoatr spped is: " + animator.speed);
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
            EventManager.PlayerGotHit += StopMonsterMovement;
            EventManager.PlayerKeepHitting += AddMonsterHit;
            EventManager.PlayerStopHitting += UnhookMonster;
            EventManager.InitiatePlayerRespawn += RestartMonsterPos;
            EventManager.FinishRespawn += RestartMonsterSpeed;
            EventManager.FinishGameStart += ChangeToRunning;
        }

        private void OnDisable()
        {
            EventManager.HitMonster -= ChangeStateToHit;
            EventManager.PlayerGotHit -= StopMonsterMovement;
            EventManager.PlayerKeepHitting -= AddMonsterHit;
            EventManager.PlayerStopHitting -= UnhookMonster;
            EventManager.InitiatePlayerRespawn -= RestartMonsterPos;
            EventManager.FinishRespawn -= RestartMonsterSpeed;
            EventManager.FinishGameStart -= ChangeToRunning;

        }

        private void ChangeStateToHit(GameObject hitGameObject)
        {
            if (this.gameObject.Equals(hitGameObject))
            {
                _isHooked = true;
                monsterHealth.AddHit();
                monsterMovement.SetSpeed(0);
                _currentState = MonsterState.GotHit;
            }
        }

        private void StopMonsterMovement(bool stop)
        {
            if (stop)
            {
                monsterMovement.SetSpeed(0);
            }
        }

        private void AddMonsterHit(int hit)
        {
            if (_isHooked)
            {
                monsterHealth.AddHit();
            }
        }

        private void UnhookMonster(bool stop)
        {
            _isHooked = false;
        }

        private void RestartMonsterPos(bool restart)
        {
            transform.position = _initialMonsterPosition;
        }

        private void RestartMonsterSpeed(bool restart)
        { 
            monsterMovement.SetSpeed(_initialMonsterSpeed);
        }
        
        private void ChangeToRunning(bool obj)
        {
            animator.speed = _initialAnimationSpeed;
            _currentState = MonsterState.Running;
        }
    }
}
