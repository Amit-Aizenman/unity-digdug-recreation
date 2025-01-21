using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerTileMaker : MonoBehaviour
{
    [SerializeField] private Tilemap normalTilemap;
    [SerializeField] private List<Tilemap> dugTilemaps;
    [SerializeField] private List<TileBase> dugTiles;
    private Vector3Int _previousDugTilePos;
    private string _previousDirection = "right";
    private string _currentDirection = "right";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _currentDirection = PlayerMovement.GetDirection();
        dugTilemaps[0].GetTile(dugTilemaps[0].WorldToCell(transform.position));
        if (!dugTilemaps[0]
                .GetTile(dugTilemaps[0].WorldToCell(transform.position))) // if no dug tile in player position
        {
            if (_previousDirection.Equals(_currentDirection))
            {
                if (_currentDirection.Equals("right") || _currentDirection.Equals("left"))
                {
                    dugTilemaps[0].SetTile(dugTilemaps[0].WorldToCell(transform.position), dugTiles[0]);
                }
                else
                {
                    dugTilemaps[0].SetTile(dugTilemaps[0].WorldToCell(transform.position), dugTiles[1]);
                }
            }
            else
            {
                if (_currentDirection.Equals("right"))
                {
                    if (_previousDirection.Equals("left"))
                    {
                        dugTilemaps[0].SetTile(dugTilemaps[0].WorldToCell(transform.position), dugTiles[0]);
                        _previousDirection = _currentDirection;
                    }
                    else if (_previousDirection.Equals("up"))
                    {
                        dugTilemaps[0].SetTile(_previousDugTilePos, dugTiles[7]);
                        _previousDirection = _currentDirection;
                    }
                    else if (_previousDirection.Equals("down"))
                    {
                        dugTilemaps[0].SetTile(_previousDugTilePos, dugTiles[8]);
                        _previousDirection = _currentDirection;
                    }
                }
                else if (_currentDirection.Equals("left"))
                {
                    if (_previousDirection.Equals("right"))
                    {
                        dugTilemaps[0].SetTile(dugTilemaps[0].WorldToCell(transform.position), dugTiles[0]);
                        _previousDirection = _currentDirection;
                    }
                    else if (_previousDirection.Equals("up"))
                    {
                        dugTilemaps[0].SetTile(_previousDugTilePos, dugTiles[6]);
                        _previousDirection = _currentDirection;
                    }
                    else if (_previousDirection.Equals("down"))
                    {
                        dugTilemaps[0].SetTile(_previousDugTilePos, dugTiles[9]);
                        _previousDirection = _currentDirection;
                    }
                }
                else if (_currentDirection.Equals("up"))
                {
                    if (_previousDirection.Equals("left"))
                    {
                        dugTilemaps[0].SetTile(_previousDugTilePos, dugTiles[8]);
                        _previousDirection = _currentDirection;
                    }
                    else if (_previousDirection.Equals("right"))
                    {
                        dugTilemaps[0].SetTile(_previousDugTilePos, dugTiles[9]);
                        _previousDirection = _currentDirection;
                    }
                    else if (_previousDirection.Equals("down"))
                    {
                        dugTilemaps[0].SetTile(dugTilemaps[0].WorldToCell(transform.position), dugTiles[1]);
                        _previousDirection = _currentDirection;
                    }
                }
                else if (_currentDirection.Equals("down"))
                {
                    if (_previousDirection.Equals("left"))
                    {
                        dugTilemaps[0].SetTile(_previousDugTilePos, dugTiles[7]);
                        _previousDirection = _currentDirection;
                    }
                    else if (_previousDirection.Equals("up"))
                    {
                        dugTilemaps[0].SetTile(dugTilemaps[0].WorldToCell(transform.position), dugTiles[1]);
                        _previousDirection = _currentDirection;
                    }
                    else if (_previousDirection.Equals("right"))
                    {
                        dugTilemaps[0].SetTile(_previousDugTilePos, dugTiles[6]);
                        _previousDirection = _currentDirection;
                    }
                }
            }
        }
        else
        {
            _previousDugTilePos = dugTilemaps[0].WorldToCell(transform.position);
        }
    }

    private void TileLogic()
    {
        
    }
    
}
