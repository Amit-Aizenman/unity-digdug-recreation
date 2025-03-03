using Managers;

namespace Player.StateMachine.Concrete_States
{
    public class PlayerHurtState : BaseState
    {
        public override void OnEnter(PlayerStateManager stateManager, SoundManager soundManager)
        {
            base.OnEnter(stateManager, soundManager);
        }

        public override void UpdateState()
        {
            
        }
    }
}