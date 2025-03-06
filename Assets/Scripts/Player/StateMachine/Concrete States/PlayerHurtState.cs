using Managers;

namespace Player.StateMachine.Concrete_States
{
    public class PlayerHurtState : PlayerBaseState
    {
        public override void EnterState(PlayerStateManager stateManager, SoundManager soundManager)
        {
            base.EnterState(stateManager, soundManager);
        }

        public override void UpdateState()
        {
            
        }
    }
}