using System.Collections;
using Managers;
using UnityEngine;

namespace Player
{
    public enum PlayerState
    {
        Running,
        Attacking,
        Hitting,
        Dying,
    }

    public class PlayerStateMachine : MonoBehaviour
    {
        private static readonly int Horizontal = Animator.StringToHash("horizontal");
        private static readonly int Vertical = Animator.StringToHash("vertical");
        private static readonly int Running = Animator.StringToHash("Running");
        private static readonly int Attacking = Animator.StringToHash("Attacking");
        private static readonly int Dying = Animator.StringToHash("Dying");
        private static readonly int Hitting = Animator.StringToHash("Hitting");
        private static readonly int Pushing = Animator.StringToHash("Pushing");
    
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerMovement playerMovement;
        private PlayerState _currentState;
        private float _initialPlayerSpeed;
        private float _initialAnimationSpeed;
        private bool _isAttacking;
        private Coroutine _attackingCoroutine;
    
        //hit variables
        [SerializeField] private float stopHitTime = 1;
        private float _stopHitTimer;
        [SerializeField] private float holdSpaceTime = 1;
        private float _holdSpaceTimer;
        [SerializeField] private float pressSpaceTime = 0.8f;
        private float _pressSpaceTimer;
        

        void Start()
        {
            _initialPlayerSpeed = playerMovement.GetSpeed();
            _initialAnimationSpeed = animator.speed;
            _currentState = PlayerState.Running; // initial state
        }

        void Update()
        {
            if (_currentState != PlayerState.Hitting)
            {
            }
            switch (_currentState)
            {
                case PlayerState.Running:
                    HandleRunningState();
                    break;
                case PlayerState.Attacking:
                    HandleAttackingState();
                    break;
                case PlayerState.Hitting:
                    HandleHittingState();
                    break;
                case PlayerState.Dying:
                    HandleDyingState();
                    break;
            }
        }

        private void HandleRunningState()
        {
            //handling walking animations 
            playerMovement.SetSpeed(_initialPlayerSpeed);
            var horizontalMovement = Input.GetAxisRaw("Horizontal");
            var verticalMovement = Input.GetAxisRaw("Vertical");
            animator.SetFloat(Horizontal, horizontalMovement);
            animator.SetFloat(Vertical, verticalMovement);
            animator.speed = horizontalMovement != 0 || verticalMovement != 0 || 
                             !animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("walk") ? _initialAnimationSpeed : 0;
        
            if (Input.GetKeyDown(KeyCode.Space))
            {
                playerMovement.SetSpeed(0);
                animator.speed = _initialAnimationSpeed;
                animator.SetBool(Running, false);
                animator.SetBool(Attacking, true);
                ChangeState(PlayerState.Attacking);
            }
        }

        private void HandleAttackingState()
        {
            if (!_isAttacking)
            {
                _isAttacking = true;
                _attackingCoroutine = StartCoroutine(AttackToRunning(PlayerAttack.GetAttackTime()));
            }
        }

        private void HandleHittingState()
        {
            
            if (_isAttacking)
            {
                _isAttacking = false;
                StopCoroutine(_attackingCoroutine);
            }

            //if player pressing the movement arrows 
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                animator.SetBool(Hitting, false);
                animator.SetBool(Pushing, false);
                animator.SetBool(Running, true);
                EventManager.PlayerStopHitting?.Invoke(true);
                playerMovement.SetSpeed(_initialPlayerSpeed);
                ChangeState(PlayerState.Running);
            }
        
            //didn't hit enough time 
            else if (_stopHitTimer <= 0)
            {
                EventManager.PlayerStopHitting?.Invoke(true);
                playerMovement.SetSpeed(_initialPlayerSpeed);
                animator.SetBool(Hitting, false);
                animator.SetBool(Pushing, false);
                animator.SetBool(Running, true);
                ChangeState(PlayerState.Running);
            }
        
            //if player doesn't press anything
            else if (!Input.GetKey(KeyCode.Space))
            {
                _stopHitTimer -= Time.deltaTime;
                _holdSpaceTimer -= Time.deltaTime;
                _pressSpaceTimer -= Time.deltaTime;
            }
        
            //player press space and he can hit
            else if (Input.GetKeyDown(KeyCode.Space) && _pressSpaceTimer <=0)
            {
                _stopHitTimer = stopHitTime;
                _pressSpaceTimer = pressSpaceTime;
                _holdSpaceTimer = holdSpaceTime;
                UpdateHittingAnimation();
            }
        
            //player press or hold space
            else if (Input.GetKey(KeyCode.Space))
            {
                _stopHitTimer = stopHitTime;
                if (_holdSpaceTimer <= 0)
                {
                    UpdateHittingAnimation();
                    _pressSpaceTimer = pressSpaceTime;
                    _holdSpaceTimer = holdSpaceTime;
                }
                else
                {
                    _pressSpaceTimer -= Time.deltaTime;
                    _holdSpaceTimer -= Time.deltaTime;
                }
            }
        }

        private void HandleDyingState()
        {
            animator.speed = _initialAnimationSpeed;
            animator.SetBool(Dying, true);
            animator.SetBool(Running, false);
            Destroy(transform.parent.gameObject,2.66f);
        }


        private IEnumerator AttackToRunning(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            playerMovement.SetSpeed(_initialPlayerSpeed);
            animator.SetBool(Attacking, false);
            animator.SetBool(Running, true);
            ChangeState(PlayerState.Running);
        }
    
        private void ChangeState(PlayerState newState)
        {
            _currentState = newState;
        }


        private void PlayerGotHit(bool isHit)
        {
            animator.speed = _initialAnimationSpeed;
            playerMovement.SetSpeed(0);
            ResetAnimationBooleans();
            animator.SetBool(Dying, true);
            ChangeState(PlayerState.Dying);
        }

        private void OnEnable()
        {
            EventManager.PlayerGotHit += PlayerGotHit;
            EventManager.HitMonster += PlayerHitMonster;
            EventManager.MonsterKilled += PlayerKilledMonster;
        }

        private void OnDisable()
        {
            EventManager.PlayerGotHit -= PlayerGotHit;
            EventManager.HitMonster -= PlayerHitMonster;
            EventManager.MonsterKilled -= PlayerKilledMonster;
        }

        private void PlayerHitMonster(GameObject go)
        {
            //initiating timers
            _stopHitTimer = stopHitTime;
            _pressSpaceTimer = pressSpaceTime;
            _holdSpaceTimer = holdSpaceTime;
        
            animator.SetBool(Attacking, false);
            animator.SetBool(Hitting, true);
        
            _currentState = PlayerState.Hitting;
        }

        private void PlayerKilledMonster(bool kill)
        {
            
            EventManager.PlayerStopHitting?.Invoke(true);
            animator.SetBool(Hitting, false);
            animator.SetBool(Pushing, false);
            animator.SetBool(Running, true);
            ChangeState(PlayerState.Running);
        }

        private void ResetAnimationBooleans()
        {
            animator.SetBool(Attacking, false);
            animator.SetBool(Hitting, false);
            animator.SetBool(Dying, false);
            animator.SetBool(Running, false);
            animator.SetBool(Pushing, false);
        }

        private void UpdateHittingAnimation()
        {
            if (!animator.GetBool(Pushing)) //player hitting
            {
                EventManager.PlayerKeepHitting?.Invoke(1);
                animator.SetBool(Pushing, true);
            }
            else //player pushing
            {   
                animator.SetBool(Pushing, false);
            }
        }
    
    }
}