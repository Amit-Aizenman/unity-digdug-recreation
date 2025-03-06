using Managers;
using UnityEngine;

namespace Player.StateMachine
{
    public abstract class PlayerBaseState
    {
        protected PlayerStateManager Player;
        protected SoundManager SoundManager;

        public virtual void EnterState(PlayerStateManager player, SoundManager soundManager)
        {
            this.Player = player;
            this.SoundManager = soundManager;
        }
        public abstract void UpdateState();
    }
}
