/*using System.Collections.Generic;
using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    /*[SerializeField] private LayerMask layerMask;
    [SerializeField] private float monsterSpeed = 4f;
    private BoxCollider2D _boxCollider;
    private readonly RaycastHit2D[] _hits = new RaycastHit2D[10];
    [SerializeField] private float minTileDistance = 5f;
    private Dictionary<Vector2, int> _availableDirections = new Dictionary<Vector2, int>();
    private Vector2[] _directions = {
        Vector2.up,    // (0, 1)
        Vector2.down,  // (0, -1)
        Vector2.left,  // (-1, 0)
        Vector2.right  // (1, 0)
    };
    
     
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAvailableDirections();
        foreach (KeyValuePair<Vector2, int> entry in _availableDirections)
        {
            Debug.Log($"Key: {entry.Key}, Value: {entry.Value}");
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    private void UpdateAvailableDirections()
    {
        for (int i = 0; i < 4; i++)
        {
            var tilesInDirection = Physics2D.BoxCastNonAlloc(transform.position, _boxCollider.size,
                0f, _directions[i], _hits, Mathf.Infinity, layerMask);
            foreach (var hit in _hits)
            {
                /*if (hit.distance < minTileDistance)
                {
                    tilesInDirection--;
                }#2#
            }
            _availableDirections[_directions[i]] = tilesInDirection;
        }
    }#1#
}*/

using System.Collections.Generic;
using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float monsterSpeed = 4f;
    private BoxCollider2D _boxCollider;
    private readonly RaycastHit2D[] _hits = new RaycastHit2D[10];
    [SerializeField] private float minTileDistance = 5f;
    private Dictionary<Vector2, int> _availableDirections = new Dictionary<Vector2, int>();
    private Vector2[] _directions = {
        Vector2.up,    // (0, 1)
        Vector2.down,  // (0, -1)
        Vector2.left,  // (-1, 0)
        Vector2.right  // (1, 0)
    };
    private float maxDistance = 10f; // Example max distance

    void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        UpdateAvailableDirections();
        foreach (KeyValuePair<Vector2, int> entry in _availableDirections)
        {
            Debug.Log($"Key: {entry.Key}, Value: {entry.Value}");
        }
    }

    private void UpdateAvailableDirections()
    {
        _availableDirections.Clear(); // Reset the dictionary before updating

        for (int i = 0; i < _directions.Length; i++)
        {
            // Clear the hits array
            System.Array.Clear(_hits, 0, _hits.Length);

            // Use the collider's bounds
            Vector2 origin = _boxCollider.bounds.center;
            Vector2 boxSize = _boxCollider.bounds.size;

            Debug.Log($"Casting from {origin} with size {boxSize} in direction {_directions[i]}");

            // Perform the BoxCastNonAlloc
            int tilesInDirection = Physics2D.BoxCastNonAlloc(
                origin,
                boxSize,
                0f,
                _directions[i],
                _hits,
                maxDistance,
                layerMask
            );

            Debug.Log($"Tiles in direction {_directions[i]}: {tilesInDirection}");

            // Log each hit
            for (int j = 0; j < tilesInDirection; j++)
            {
                Debug.Log($"Hit {j}: {_hits[j].collider.name}, Distance: {_hits[j].distance}");
            }

            // Add the result to the dictionary
            _availableDirections[_directions[i]] = tilesInDirection;
        }
    }

}
