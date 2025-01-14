using System.Collections.Generic;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject topTilesTexture;
    [SerializeField] private GameObject botTilesTexture;
    [SerializeField] private GameObject midTilesTexture;
    [SerializeField] private float distanceBetweenTiles = 0.234f;
    private HashSet<Vector3> _tilesPositions;
    private Stack<GameObject> _tilesStack;
    private Vector3 playerPosition;
    void Start()
    {
        _tilesStack = new Stack<GameObject>();
        _tilesPositions = new HashSet<Vector3>(); //todo: add start positions that are empty
    }
    
    void Update()
    {
        playerPosition = player.transform.position;
        Vector3 fixedPosition = new Vector3(
            playerPosition.x - playerPosition.x % distanceBetweenTiles, 
            playerPosition.y - playerPosition.y % distanceBetweenTiles, 
            playerPosition.z - playerPosition.z % distanceBetweenTiles);
        if (!_tilesPositions.Contains(fixedPosition))
        {
            MakeTiles(fixedPosition);
        }
    }

    private void MakeTiles(Vector3 position)
    {
        GameObject topTile = Instantiate(topTilesTexture, player.transform.position + Vector3.up * 0.14f, Quaternion.identity);
        GameObject botTile = Instantiate(botTilesTexture, player.transform.position + Vector3.down * 0.1f, Quaternion.identity);
        GameObject midTile = Instantiate(midTilesTexture, player.transform.position, Quaternion.identity);
        _tilesStack.Push(topTile);
        _tilesStack.Push(botTile);
        _tilesStack.Push(midTile);
        _tilesPositions.Add(position);
    }
}
