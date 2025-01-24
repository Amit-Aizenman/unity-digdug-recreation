using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    private static String direction = "right";
    private float _horizontalMovement;
    private float _verticalMovement;
    private Vector3Int _previousTile; 
    [SerializeField] Tilemap tilemap;
    [SerializeField] int speed = 2;
    
    private Dictionary<string, Vector3> directions = new Dictionary<string, Vector3>
    {
        { "right", Vector3.right },
        { "left", Vector3.left },
        { "up", Vector3.up },
        { "down", Vector3.down },
    };
    
    void Update()
    {
        _horizontalMovement = Input.GetAxisRaw("Horizontal");
        _verticalMovement = Input.GetAxisRaw("Vertical");
        if (_horizontalMovement != 0)
        {
            var movementVector = FixedPlayerMovement(_horizontalMovement > 0 ? "right" : "left");
            transform.position += movementVector * (Time.deltaTime * speed);
        }
        else if (_verticalMovement != 0)
        {
            var movementVector = FixedPlayerMovement(_verticalMovement > 0 ? "up" : "down");
            transform.position += movementVector * (Time.deltaTime * speed);
        }
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

    public static string GetDirection()
    {
        return direction;
    }
}