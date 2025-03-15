using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Player.StateMachine.Concrete_States
{
    public class PlayerRunState : PlayerBaseState
    {
        private Vector2 _preVector;
        private readonly HashSet<Vector2> _possibleVectors = new();
        private float _adjustDistance = 0.1f;
        public override void EnterState(PlayerStateManager stateManager, SoundManager soundManager)
        {
            base.EnterState(stateManager, soundManager);
            this.SoundManager.UnPause("walkingSound");
        }

        public override void UpdateState()
        {
            var horizontalMovement = Input.GetAxisRaw("Horizontal");
            var verticalMovement = Input.GetAxisRaw("Vertical");
            var directionVector = Vector2.zero;
            
            if (horizontalMovement != 0)
                directionVector = horizontalMovement > 0? Vector2.right : Vector2.left;
            else if (verticalMovement != 0)
                directionVector = verticalMovement > 0? Vector2.up : Vector2.down;
            else
                Player.ChangeState(Player.IdleState);
            FixedMovement(directionVector);
        }

        public void FixedMovement(Vector2 directionVector)
        {
            CheckPossibleVectors();
            if (_possibleVectors.Contains(directionVector))
            {
                Player.transform.position += (Vector3) directionVector * (Time.deltaTime * Player.speed);
                _preVector = directionVector;
            }
            else
            {
                Player.transform.position += (Vector3) _preVector * (Time.deltaTime * Player.speed);
                AdjustPosition(_preVector);
            }
        }
        private void CheckPossibleVectors()
        {
            if (_possibleVectors.Count == 0)
            {
                _possibleVectors.Add(Vector2.up);
                _possibleVectors.Add(Vector2.down);
                _possibleVectors.Add(Vector2.right);
                _possibleVectors.Add(Vector2.left);
                return;
            }
            var cellPos = Player.tilemap.GetCellCenterLocal(Player.tilemap.WorldToCell(Player.transform.position));
            if (Player.transform.position.x == cellPos.x)
            {
                _possibleVectors.Add(Vector2.up);
                _possibleVectors.Add(Vector2.down);
            }
            else
            {
                _possibleVectors.Remove(Vector2.up);
                _possibleVectors.Remove(Vector2.down);
            }
            if (Player.transform.position.y == cellPos.y)
            {
                _possibleVectors.Add(Vector2.right);
                _possibleVectors.Add(Vector2.left);
            }
            else
            {
                _possibleVectors.Remove(Vector2.right);
                _possibleVectors.Remove(Vector2.left);
            }
        }

        private void AdjustPosition(Vector2 movementVector)
        {
            var cellPos = Player.tilemap.GetCellCenterLocal(Player.tilemap.WorldToCell(Player.transform.position));
            if (movementVector == Vector2.right)
            {
                if (cellPos.x - Player.transform.position.x <= _adjustDistance && cellPos.x - Player.transform.position.x >0)
                {
                    Debug.Log("adjustgin");
                    Player.transform.position = new Vector3(cellPos.x, cellPos.y, Player.transform.position.z);
                }
            }
            else if (movementVector == Vector2.left)
            {
                if (Player.transform.position.x - cellPos.x <= _adjustDistance && Player.transform.position.x - cellPos.x >0)
                {
                    Player.transform.position = cellPos;
                }
            }
            else if (movementVector == Vector2.up)
            {
                if (cellPos.y - Player.transform.position.y <= _adjustDistance && cellPos.y - Player.transform.position.y  >=0)
                {
                    Player.transform.position = cellPos;
                }
            }
            else if (movementVector == Vector2.down)
            {
                if (Player.transform.position.y - cellPos.y <= _adjustDistance && Player.transform.position.y - cellPos.y >=0)
                {
                    Player.transform.position = cellPos;
                }
            }
        } 
    }
}