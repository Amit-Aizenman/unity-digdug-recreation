using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GizmoRedDot : MonoBehaviour
{
    public Transform movingObject; // Assign the moving object in the Inspector
    public float dotSize = 0.2f;   // Size of the red dot
    public Tilemap Tilemap;

    void OnDrawGizmos()
    {
        if (movingObject != null)
        {
            Gizmos.color = Color.red; // Set gizmo color to red
            Gizmos.DrawSphere(movingObject.position, dotSize); // Draw a sphere at the object's position
        }

        var previousTile = PlayerTileMaker.GetPreviousTile();
        var pos = Tilemap.GetCellCenterWorld(previousTile);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(pos, dotSize);
    }
}