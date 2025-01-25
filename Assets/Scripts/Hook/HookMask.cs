using UnityEngine;

public class HookMask : MonoBehaviour
{
    [SerializeField] private float speed = 10f; // Speed of the hook
    [SerializeField] float lifetime = 0.5f; // Time before the hook is destroyed

    void Start()
    {
        Destroy(gameObject, lifetime); //todo:change this based on hook 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
