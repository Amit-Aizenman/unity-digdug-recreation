using System.Collections;
using Managers;
using Player;
using UnityEngine;

public enum PlayerState
{
    Running,
    Attacking,
    Hitting,
    Dying,
}

public class PlayerStateMachine : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement playerMovement;
    private static readonly int Horizontal = Animator.StringToHash("horizontal");
    private static readonly int Vertical = Animator.StringToHash("vertical");
    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int Attacking = Animator.StringToHash("Attacking");
    private static readonly int Dying = Animator.StringToHash("Dying");
    private static readonly int Hitting = Animator.StringToHash("Hitting");
    private PlayerState currentState;
    private float _initialPlayerSpeed;
    private float _initialAnimationSpeed;
    private bool _isRunning = true;
    private bool _isAttacking = false;
    private bool _isHitting = false;
    private bool _isDying = false;


    void Start()
    {
        _initialPlayerSpeed = playerMovement.GetSpeed();
        _initialAnimationSpeed = animator.speed;
        currentState = PlayerState.Running; // initial state
    }

    void Update()
    {
        switch (currentState)
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
        var horizontalMovement = Input.GetAxisRaw("Horizontal");
        var verticalMovement = Input.GetAxisRaw("Vertical");
        animator.SetFloat(Horizontal, horizontalMovement);
        animator.SetFloat(Vertical, verticalMovement);
        animator.speed = horizontalMovement != 0 || verticalMovement != 0 || 
                         !animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("walk") ? _initialAnimationSpeed : 0;
        
        
        if (_isDying) // todo: find out where do i change it 
        {
            _isRunning = false;
            animator.SetBool(Running, false);
            animator.SetBool(Dying, true);
            animator.speed = _initialAnimationSpeed;
            ChangeState(PlayerState.Dying);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            playerMovement.SetSpeed(0);
            animator.speed = _initialAnimationSpeed;
            _isRunning = false;
            _isAttacking = true;
            animator.SetBool(Running, false);
            animator.SetBool(Attacking, true);
            ChangeState(PlayerState.Attacking);
        }
    }

    private void HandleAttackingState()
    {
        if (_isHitting)
        {
            _isAttacking = false;
            _isHitting = true;
            animator.SetBool(Attacking, false);
            animator.SetBool(Hitting, true);
            ChangeState(PlayerState.Hitting);
        }
        else
        {
            StartCoroutine(AttackToRunning(1));
        }
    }

    private void HandleHittingState()
    {
        animator.SetTrigger("jump");
        //rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // Transition to idle after landing
        /*if (rb.velocity.y <= 0)
        {
            currentState = PlayerState.Idle;
        }*/
    }

    private void HandleDyingState()
    {
        animator.speed = _initialAnimationSpeed;
        animator.SetTrigger(Dying);
        Destroy(transform.parent.gameObject,2.34f);
    }


    private IEnumerator AttackToRunning(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _isAttacking = false;
        _isRunning = true;
        playerMovement.SetSpeed(_initialPlayerSpeed);
        animator.SetBool(Attacking, false);
        animator.SetBool(Running, true);
        ChangeState(PlayerState.Running);
    }
    
    private void ChangeState(PlayerState newState)
    {
        currentState = newState;
    }


    private void PlayerGotHit(bool isHit)
    {
        if (isHit)
        {
            playerMovement.SetSpeed(0);
            ChangeState(PlayerState.Dying);
        }
    }
    private void OnEnable()
    {
        EventManager.HitPlayer += PlayerGotHit;
    }

    private void OnDisable()
    {
        EventManager.HitPlayer -= PlayerGotHit;
    }
}
