using Managers;

namespace Player.StateMachine.Concrete_States
{
    public class PlayerRunState : BaseState
    {
        public override void OnEnter(PlayerStateManager stateManager, SoundManager soundManager)
        {
            base.OnEnter(stateManager, soundManager);
            this.SoundManager.UnPause("walkingSound");
        }

        public override void UpdateState()
        {
            
        }
    }
}