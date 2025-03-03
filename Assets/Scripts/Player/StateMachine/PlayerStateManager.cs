using Managers;
using Player.StateMachine;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    private BaseState _currentState;
    [SerializeField] private SoundManager soundManager;

    [SerializeField] private BaseState _startState;
    [SerializeField] private BaseState _idleState;
    [SerializeField] private BaseState _runState;
    [SerializeField] private BaseState _hurtState;
    [SerializeField] private BaseState _attackState;
    [SerializeField] private BaseState _hitState;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentState = _startState;
    }

    // Update is called once per frame
    void Update()
    {
        _currentState.UpdateState();
    }

    void ChangeState(BaseState state)
    {
        this._currentState = state;
        _currentState.OnEnter(this, soundManager);
    }
}
