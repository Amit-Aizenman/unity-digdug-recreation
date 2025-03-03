using Managers;

namespace Player.StateMachine.Concrete_States
{
    public class PlayerIdleState : BaseState
    {
        public override void OnEnter(PlayerStateManager stateManager, SoundManager soundManager)
        {
            base.OnEnter(stateManager, soundManager);
            this.SoundManager.Pause("walkingSound");

        }
        public override void UpdateState(){}
    }
}
