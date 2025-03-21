using System;
using System.Collections.Generic;
using Managers;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private static string direction = "right";
        private float _horizontalMovement;
        private float _verticalMovement;
        private Vector3Int _previousTile; 
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private float speed = 2;
        private bool _canMove;
    
        private readonly Dictionary<string, Vector3> _directions = new()
        {
            { "right", Vector3.right },
            { "left", Vector3.left },
            { "up", Vector3.up },
            { "down", Vector3.down },
        };

        void Update()
        {
            if (_canMove)
            {
                _horizontalMovement = Input.GetAxisRaw("Horizontal");
                _verticalMovement = Input.GetAxisRaw("Vertical");
                if (_horizontalMovement != 0)
                {
                    FindAnyObjectByType<SoundManager>().UnPause("walkingSound");
                    var movementVector = FixedPlayerMovement(_horizontalMovement > 0 ? "right" : "left");
                    if (InBounds(transform.position + movementVector * (Time.deltaTime * speed)))
                    {
                        transform.position += movementVector * (Time.deltaTime * speed);
                    }
                }
                else if (_verticalMovement != 0)
                {
                    FindAnyObjectByType<SoundManager>().UnPause("walkingSound");
                    var movementVector = FixedPlayerMovement(_verticalMovement > 0 ? "up" : "down");
                    if (InBounds(transform.position + movementVector * (Time.deltaTime * speed)))
                    {
                        transform.position += movementVector * (Time.deltaTime * speed);
                    }
                }
                else
                {
                    FindAnyObjectByType<SoundManager>().Pause("walkingSound");
                }
            }
        }

        private Vector3 FixedPlayerMovement(String wantedDirection)
        {
            var currentCellPos = tilemap.GetCellCenterWorld(tilemap.WorldToCell(transform.position));
            if (currentCellPos == transform.position)
            {
                direction = wantedDirection;
                return _directions[wantedDirection];
            }

            if (wantedDirection.Equals("right") || wantedDirection.Equals("left"))
            {
                if (math.abs(transform.position.y - currentCellPos.y) < 0.1f)
                {
                    transform.position = new Vector3(transform.position.x, currentCellPos.y,transform.position.z);
                    direction = wantedDirection;
                    return _directions[wantedDirection];
                }
                return _directions[direction];
            }
            if (math.abs(transform.position.x - currentCellPos.x) < 0.1f)
            {
                transform.position = new Vector3(currentCellPos.x, transform.position.y,transform.position.z);
                direction = wantedDirection;
                return _directions[wantedDirection];
            }
            return _directions[direction];
        }

        public static string GetDirection()
        {
            return direction;

        }

        public static void SetDirection(string newDirection)
        {
            direction = newDirection;
        }

        private bool InBounds(Vector3 pos)
        {
            if (pos.x < -5.5f)
            {
                transform.position = new Vector3(-5.5f, transform.position.y, transform.position.z);
                return false;
            }
            if (pos.x > 7.5f)
            {
                transform.position = new Vector3(7.5f, transform.position.y, transform.position.z);
                return false;
            }
            if (pos.y < -11.5f)
            {
                transform.position = new Vector3(transform.position.x, -11.5f, transform.position.z);
                return false;
            }
            if (pos.y > 2.5f)
            {
                transform.position = new Vector3(transform.position.x, 2.5f, transform.position.z);
                return false;
            }
            return true;
        }

        public void SetSpeed(float newSpeed)
        {
            this.speed = newSpeed;
        }

        public float GetSpeed()
        {
            return speed;
        }
        
        public void SetPosition(Vector3 pos)
        {
            transform.position = pos;
        }

        private void OnEnable()
        {
            EventManager.FinishGameStart += ChangeMoveFlag;
            EventManager.FinishLevel += ChangeMoveFlag;
        }
        
        private void OnDisable()
        {
            EventManager.FinishGameStart -= ChangeMoveFlag;
            EventManager.FinishLevel -= ChangeMoveFlag;
        }

        public void ChangeMoveFlag(bool obj)
        {
            _canMove = !_canMove;
        }
    }
}