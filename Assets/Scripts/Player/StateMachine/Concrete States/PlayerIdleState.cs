using Managers;
using UnityEngine;

namespace Player.StateMachine.Concrete_States
{
    public class PlayerIdleState : PlayerBaseState
    {
        public override void EnterState(PlayerStateManager stateManager, SoundManager soundManager)
        {
            base.EnterState(stateManager, soundManager);
            this.SoundManager.Pause("walkingSound");

        }

        public override void UpdateState()
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                Player.ChangeState(Player.RunState);
            }
        }
    }
}
