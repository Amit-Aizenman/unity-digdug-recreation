using System;
using UnityEngine;

public class HookMovement : MonoBehaviour
{
    [SerializeField] private float hookSpeed = 4; 
    [SerializeField] private float maxDistance = 1.5f;
    private Vector3 _initialPosition;

    private void Start()
    {
        _initialPosition = transform.position;
    }

    void Update()
    {
        transform.position += Vector3.right * (Time.deltaTime * hookSpeed);
        if (transform.position.x > _initialPosition.x + maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("")) ;
    }
    
}
