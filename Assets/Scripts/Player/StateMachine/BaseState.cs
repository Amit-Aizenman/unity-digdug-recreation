using Managers;

namespace Player.StateMachine
{
    public abstract class BaseState
    {
        protected PlayerStateManager StateManager;
        protected SoundManager SoundManager;

        public virtual void OnEnter(PlayerStateManager stateManager, SoundManager soundManager)
        {
            this.StateManager = stateManager;
            this.SoundManager = soundManager;
        }
        public abstract void UpdateState();
        
    }
}
