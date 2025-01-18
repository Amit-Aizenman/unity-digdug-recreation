using System;
using System.Drawing;
using NUnit.Framework.Constraints;
using UnityEngine;
using Object = System.Object;

public class PlayerMovement : MonoBehaviour
{
    private static readonly int Horizontal = Animator.StringToHash("horizontal");
    private static readonly int Vertical = Animator.StringToHash("vertical");
    private float _horizontalMovement;
    private float _verticalMovement;
    private float _initialAnimationSpeed;
    [SerializeField] int speed = 2;
    [SerializeField] Animator animator;
    private static String direction = "right";

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
            animator.speed = _initialAnimationSpeed;
            transform.position += Vector3.right * (Time.deltaTime * _horizontalMovement * speed);
            if (_horizontalMovement > 0)
                direction = "right";
            else
                direction = "left";
        }
        else if (_verticalMovement != 0)
        {
            animator.speed = _initialAnimationSpeed;
            transform.position += Vector3.up * (Time.deltaTime * _verticalMovement * speed);
            if (_verticalMovement > 0)
                direction = "up";
            else
                direction = "down";
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
}