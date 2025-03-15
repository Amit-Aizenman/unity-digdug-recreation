using System.Collections;
using Managers;
using UnityEngine;

namespace Player.StateMachine.Concrete_States
{
    public class PlayerStartState : PlayerBaseState
    {
        private Vector3 _startPosition = new Vector3(3, 2.5f, 0);
        private Vector3 _endPosition = new Vector3(0.5f, -4.5f, 0);
        private float _secondToFinish = 2f;
        public override void EnterState(PlayerStateManager stateManager, SoundManager soundManager)
        {
            base.EnterState(stateManager, soundManager);
            Player.transform.position = _startPosition;
        }

        public override void UpdateState()
        {
            if (Player.transform.position.x != _endPosition.x)
                Player.RunState.FixedMovement(Vector2.left);

            else if (Player.transform.position.y != _endPosition.y)
                Player.RunState.FixedMovement(Vector2.down);
            else
            {
                var stopStart = StopStart(_secondToFinish);
            }
        }
        
        
        private IEnumerator StopStart (float seconds)
        {
            yield return new WaitForSeconds(seconds);
            EventManager.FinishGameStart?.Invoke(true);
        }
    }
}