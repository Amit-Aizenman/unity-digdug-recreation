using System;
using System.Collections.Generic;
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

    private Dictionary<string, int> _moveValues = new Dictionary<string, int>
    {
        { "up", 0 },
        { "right", 90 },
        { "down", 180 },
        { "left", 270 },
    };
    
    void Update()
    {
        _currentDirection = PlayerMovement.GetDirection();
        TileLogic(0);
    }

    void PlaceTile(Tilemap tilemap, TileBase tile, float angle, Vector3 position)
    {
        Vector3Int tilePos = tilemap.WorldToCell(position);
        tilemap.SetTile(tilePos, tile);
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, -angle));
        tilemap.SetTransformMatrix(tilePos, rotationMatrix);
        tilemap.RefreshTile(tilePos);
    }

    private void TileLogic(int i)
    {
        if (!dugTilemaps[i]
                .GetTile(dugTilemaps[i].WorldToCell(transform.position))) // if no dug tile in player position
        {
            if (_previousDirection.Equals(_currentDirection) ||
                Math.Abs(_moveValues[_previousDirection] - _moveValues[_currentDirection]) == 180) //going to the same axis
            {
                _previousDirection = _currentDirection;
                if (PlayerBeforeTileCenter())
                {
                    PlaceTile(dugTilemaps[i], dugTiles[0], _moveValues[_previousDirection],transform.position);
                    
                }
                else
                {
                    PlaceTile(dugTilemaps[i], dugTiles[1], _moveValues[_previousDirection],transform.position);
                }
            }
            else //going to vertical direction
            {
                float movementValue = (_moveValues[_currentDirection] + _moveValues[_previousDirection]) % 360;
                if (movementValue > 180 )
                {
                    if (_moveValues[_previousDirection] - _moveValues[_currentDirection] < 0)
                    {
                        PlaceTile(dugTilemaps[i], dugTiles[2], 0, PreviousTile());
                        _previousDirection = _currentDirection;
                    }
                    else
                    {
                        PlaceTile(dugTilemaps[i], dugTiles[2], 180, PreviousTile());
                        _previousDirection = _currentDirection;
                    }
                }
                else
                {
                    if (_previousDirection.Equals("right") || _previousDirection.Equals("down"))
                    {
                        PlaceTile(dugTilemaps[i], dugTiles[2], 90, PreviousTile());
                        _previousDirection = _currentDirection;
                    }
                    else
                    {
                        PlaceTile(dugTilemaps[i], dugTiles[2], 270, PreviousTile());
                        _previousDirection = _currentDirection;
                    }
                }
            }
        }
    }

    

    private bool PlayerBeforeTileCenter()
    {
        int directionValue = _currentDirection.Equals("right") || _currentDirection.Equals("up")? 1 : -1;
        Vector3Int cellPos = normalTilemap.WorldToCell(transform.position);
        Vector3 centerCellPos = normalTilemap.GetCellCenterWorld(cellPos);
        Vector3 distanceFromCenter = centerCellPos - transform.position;
        if (distanceFromCenter.x != 0)
        {
            return directionValue * distanceFromCenter.x >= 0;
        }
        return directionValue * distanceFromCenter.y >= 0;
            
    }

    private Vector3 PreviousTile()
    {
        Grid grid = normalTilemap.layoutGrid;
        float cellSize = grid.cellSize.x;
        
        if (_currentDirection.Equals("right"))
            return transform.position - Vector3.right * cellSize;
        if (_currentDirection.Equals("up"))
            return transform.position - Vector3.up * cellSize;
        if (_currentDirection.Equals("down"))
            return transform.position - Vector3.down * cellSize;
        return transform.position - Vector3.left * cellSize;
    }
}