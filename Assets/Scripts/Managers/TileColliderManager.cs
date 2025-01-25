using UnityEngine;
using UnityEngine.Tilemaps;

public class TileColliderManager : MonoBehaviour
{

    [SerializeField] private Tilemap tilemap; // Your Tilemap
    [SerializeField] private GameObject colliderPrefab; // A prefab that holds the collider

    void Start()
    {
        SetCollidersForSpecificPositions();
    }

    // This function allows you to add colliders at specific positions in the Tilemap
    public void SetCollidersForSpecificPositions()
    {
        // Define the list of positions where you want to add the colliders
        Vector3Int[] positionsToAddColliders = new Vector3Int[]
        {
            new Vector3Int(2, 2, 0), // Position (2,2) in cell coordinates
            new Vector3Int(4, 3, 0), // Position (4,3)
            new Vector3Int(6, 5, 0), // Position (6,5)
        };

        foreach (Vector3Int position in positionsToAddColliders)
        {
            // Check if a tile exists at the given position
            TileBase tile = tilemap.GetTile(position);
            if (tile != null) // If a tile exists at the position, create a collider
            {
                // Convert cell position to world position
                Vector3 worldPos = tilemap.CellToWorld(position);

                // Instantiate a new collider (you can use a BoxCollider2D or any other collider)
                GameObject colliderObject = Instantiate(colliderPrefab, worldPos, Quaternion.identity);

                // Optionally, adjust the collider size (based on tile size)
                BoxCollider2D boxCollider = colliderObject.GetComponent<BoxCollider2D>();
                if (boxCollider != null)
                {
                    boxCollider.size = tilemap.cellSize; // Match collider size to tile size
                }
            }
        }
    }
}
