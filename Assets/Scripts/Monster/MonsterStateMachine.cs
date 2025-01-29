using Managers;
using UnityEngine;

namespace Monster
{
    public enum MonsterState
    {   
        Starting,
        Running,
        HitByPlayer,
        HitByRock,
    }

    public class MonsterStateMachine : MonoBehaviour
    {
        private static readonly int Hits = Animator.StringToHash("Hits");
        private static readonly int Direction = Animator.StringToHash("Direction");
        private static readonly int RockHit = Animator.StringToHash("RockHit");
        [SerializeField] private Animator animator;
        [SerializeField] private MonsterMovement monsterMovement;
        [SerializeField] private MonsterHealth monsterHealth;
        [SerializeField] private Collider2D monsterCollider;
        private MonsterState _currentState;
        private float _initialMonsterSpeed;
        private float _initialAnimationSpeed;
        private Vector3 _initialMonsterPosition;
        private bool _isHooked;
        private bool _rockStopFall;
        private bool _isDestroyed;

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
                case MonsterState.HitByPlayer:
                    HandleHitByPlayerState();
                    break;
                case MonsterState.Starting:
                    HandleStartingState();
                    break;
                case MonsterState.HitByRock:
                    HandleHitByRockState();
                    break;
            }
        }

        private void HandleHitByRockState()
        {
            monsterCollider.enabled = false;
            monsterMovement.ChangeRockFlag();
            monsterMovement.SetSpeed(0);
            if (!_rockStopFall)
            {
                transform.position += Vector3.down * (Rock.RockStateMachine.RockSpeed * Time.deltaTime);
            }
            else
            {
                if (!_isDestroyed)
                {
                    _isDestroyed = true;
                    EventManager.MonsterKilled?.Invoke(true);
                    Destroy(gameObject, 2f);
                }
            }
        }

        private void HandleStartingState()
        {
            monsterCollider.enabled = true;
            animator.speed = 0;
        }

        private void HandleRunningState()
        {
            monsterCollider.enabled = true;
            if (monsterMovement.GetCurrentDirection() == Vector3.right)
                animator.SetBool(Direction, true);
            else if (monsterMovement.GetCurrentDirection() == Vector3.left)
                animator.SetBool(Direction, false);
        }

        private void HandleHitByPlayerState()
        {
            monsterCollider.enabled = false;
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
            EventManager.RockStoppedFalling += DestroyMonster;
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
            EventManager.RockStoppedFalling -= DestroyMonster;

        }

        private void DestroyMonster(bool obj)
        {
            if (_currentState == MonsterState.HitByRock)
            {
                _rockStopFall = true;
            }
        }

        private void ChangeStateToHit(GameObject hitGameObject)
        {
            if (this.gameObject.Equals(hitGameObject))
            {
                _isHooked = true;
                monsterHealth.AddHit();
                monsterMovement.SetSpeed(0);
                _currentState = MonsterState.HitByPlayer;
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

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Rock"))
            {
                FindAnyObjectByType<SoundManager>().Play("hitWithRock");
                animator.SetBool(RockHit, true);
                _currentState = MonsterState.HitByRock;
            }
        }
        
        
    }
}
