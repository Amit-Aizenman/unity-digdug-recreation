using UnityEngine;

public enum PlayerState
{
    Running,
    Attacking,
    Hitting,
    Dying,
}

public class PlayerAnimationStateMachine : MonoBehaviour
{
    private static readonly int Horizontal = Animator.StringToHash("horizontal");
    private static readonly int Vertical = Animator.StringToHash("vertical");
    private PlayerState currentState;
    [SerializeField] private Animator animator;
    private float _initialAnimationSpeed;

    void Start()
    {
        _initialAnimationSpeed = animator.speed;
        currentState = PlayerState.Running; // Set initial state
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

    void HandleRunningState()
    {

        float _horizontalMovement = Input.GetAxisRaw("Horizontal");
        float _verticalMovement = Input.GetAxisRaw("Vertical");
        animator.SetFloat(Horizontal, _horizontalMovement);
        animator.SetFloat(Vertical, _verticalMovement);
        if (_horizontalMovement != 0 || _verticalMovement != 0)
        {
            animator.speed = _initialAnimationSpeed;
        }
        else
        {
            animator.speed = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentState = PlayerState.Attacking;
        }
    }

    void HandleAttackingState()
    {
        animator.SetTrigger("attack");

        // Attack logic (e.g., deal damage, spawn hitbox, etc.)
        Debug.Log("Player is attacking!");

        // Return to idle after attack animation ends
    }

    void HandleHittingState()
    {
        animator.SetTrigger("jump");
        //rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // Transition to idle after landing
        /*if (rb.velocity.y <= 0)
        {
            currentState = PlayerState.Idle;
        }*/
    }

    void HandleDyingState()
    {
        animator.SetTrigger("stunned");

        // Prevent all input during this state
        Debug.Log("Player is stunned!");
        // Transition out of stunned after some time (example: 2 seconds)
        Invoke("RecoverFromStun", 2f);
    }
}
