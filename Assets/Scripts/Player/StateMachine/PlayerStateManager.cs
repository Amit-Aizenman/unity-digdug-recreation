using Managers;
using Player.StateMachine.Concrete_States;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Player.StateMachine
{
    public class PlayerStateManager : MonoBehaviour
    {
        private PlayerBaseState _currentState;
        public Tilemap tilemap; 
        [SerializeField] private SoundManager soundManager;
        [SerializeField] public int speed;

        
        public PlayerStartState StartState = new PlayerStartState();
        public PlayerIdleState IdleState = new PlayerIdleState();
        public PlayerRunState RunState = new PlayerRunState();
        public PlayerHurtState HurtState = new PlayerHurtState();
        public PlayerAttackState AttackState = new PlayerAttackState();
        public PlayerHitState HitState = new PlayerHitState();


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _currentState = StartState;
            _currentState.EnterState(this, soundManager);
        }

        // Update is called once per frame
        void Update()
        {
            _currentState.UpdateState();
        }

        public void ChangeState(PlayerBaseState state)
        {
            this._currentState = state;
            _currentState.EnterState(this, soundManager);
        }
    }
}
