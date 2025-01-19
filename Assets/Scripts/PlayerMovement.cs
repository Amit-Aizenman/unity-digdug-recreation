using System;
using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework.Constraints;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = System.Object;

public class PlayerMovement : MonoBehaviour
{
    private static readonly int Horizontal = Animator.StringToHash("horizontal");
    private static readonly int Vertical = Animator.StringToHash("vertical");
    
    private static String direction = "right";
    private float _horizontalMovement;
    private float _verticalMovement;
    private float _initialAnimationSpeed;
    [SerializeField] Tilemap tilemap;
    [SerializeField] int speed = 2;
    [SerializeField] Animator animator;
    
    private Dictionary<string, Vector3> directions = new Dictionary<string, Vector3>
    {
        { "right", Vector3.right },
        { "left", Vector3.left },
        { "up", Vector3.up },
        { "down", Vector3.down },
    };

    void Start()
    {
        _initialAnimationSpeed = animator.speed;
    }
    void Update()
    {
        _horizontalMovement = Input.GetAxisRaw("Horizontal");
        _verticalMovement = Input.GetAxisRaw("Vertical");
        animator.SetFloat(Horizontal, _horizontalMovement);
        animator.SetFloat(Vertical, _verticalMovement);
        if (_horizontalMovement != 0)
        {
            var movementVector = FixedPlayerMovement(_horizontalMovement > 0 ? "right" : "left");
            animator.speed = _initialAnimationSpeed;
            transform.position += movementVector * (Time.deltaTime * speed);
        }
        else if (_verticalMovement != 0)
        {
            var movementVector = FixedPlayerMovement(_verticalMovement > 0 ? "up" : "down");
            animator.speed = _initialAnimationSpeed;
            transform.position += movementVector * (Time.deltaTime * speed);
        }
        else
            animator.speed = 0;
    }

    public static String GetPlayerDirection()
    {
        return direction;
    }

    public void StepSound()
    {
        //todo
    }

    private Vector3 FixedPlayerMovement(String wantedDirection)
    {
        var currentCellPos = tilemap.GetCellCenterWorld(tilemap.WorldToCell(transform.position));
        if (currentCellPos == transform.position)
        {
            direction = wantedDirection;
            return directions[wantedDirection];
        }

        if (wantedDirection.Equals("right") || wantedDirection.Equals("left"))
        {
            if (math.abs(transform.position.y - currentCellPos.y) < 0.1f)
            {
                transform.position = new Vector3(transform.position.x, currentCellPos.y,transform.position.z);
                direction = wantedDirection;
                return directions[wantedDirection];
            }
            return directions[direction];
        }
        if (math.abs(transform.position.x - currentCellPos.x) < 0.1f)
        {
            transform.position = new Vector3(currentCellPos.x, transform.position.y,transform.position.z);
            direction = wantedDirection;
            return directions[wantedDirection];
        }
        return directions[direction];
    }
}