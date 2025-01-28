using System;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerTileMaker : MonoBehaviour
{
    [SerializeField] private Tilemap normalTilemap;
    [SerializeField] private List<Tilemap> dugTilemaps;
    [SerializeField] private List<TileBase> dugTiles;
    [SerializeField] private float playerOffset;
    [SerializeField] private TileManager tileManager;
    private string _previousDirection = "left";
    private string _currentDirection = "left";
    private Dictionary<Vector3Int, (string, float)> _dugTileDictionary = new();
    private static Vector3Int _previousTile;
    private static Vector3Int _currentTile;
    private Vector3 _preTile;


    private Dictionary<string, int> _moveValues = new Dictionary<string, int>
    {
        { "up", 0 },
        { "right", 90 },
        { "down", 180 },
        { "left", 270 },
    };

    private void Start()
    {
        _previousTile = dugTilemaps[0].WorldToCell(transform.position);
        _currentTile = dugTilemaps[0].WorldToCell(transform.position);
    }
    private void Update()
    {
        UpdateTileHistory();
        _currentDirection = PlayerMovement.GetDirection();
        tileManager.SetTile(transform.position, _previousDirection, _currentDirection, PlayerBeforeTileCenter());
        _previousDirection = _currentDirection;
    }

    private void PlaceTile(Tilemap tilemap, TileBase tile, float angle, Vector3 position)
    {
        //Debug.Log("Placing in tilemap:" + tilemap.name + ", tile: " + tile.name + ", angle: " + angle + ", position: " +
                 // ToVecInt(position));
        Vector3Int tilePos = tilemap.WorldToCell(position);
        tilemap.SetTile(tilePos, tile);
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, -angle));
        tilemap.SetTransformMatrix(tilePos, rotationMatrix);
        tilemap.RefreshTile(tilePos);
        _dugTileDictionary[ToVecInt(position)] = (tile.name, angle);
    }

    private void TileLogic(int i)
    {
        // if no dug tile in player position
        if (!dugTilemaps[i].GetTile(dugTilemaps[i].WorldToCell(transform.position)))
        {
            //going to the same axis
            if (_previousDirection.Equals(_currentDirection) ||
                Math.Abs(_moveValues[_previousDirection] - _moveValues[_currentDirection]) == 180)
            {
                SameAxisLogic(i);
            }
            
            //going to other axis
            else 
            {
                OtherAxisLogic(i);
            }
        }
        else
        {
            /*Debug.Log("In tilemap:" + dugTilemaps[i].name +
                      " , the tile: " + dugTilemaps[i].GetTile(dugTilemaps[i].WorldToCell(transform.position))
                      + "is already there.");*/
        }
    }

    private bool PlayerBeforeTileCenter()
    {
        int directionValue = _currentDirection.Equals("right") || _currentDirection.Equals("up") ? 1 : -1;
        Vector3Int cellPos = normalTilemap.WorldToCell(transform.position);
        Vector3 centerCellPos = normalTilemap.GetCellCenterWorld(cellPos);
        Vector3 distanceFromCenter = centerCellPos - transform.position;
        if (distanceFromCenter.x != 0)
        {
            return directionValue * distanceFromCenter.x  >= 0;
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

    private bool IsAlreadyIn(TileBase tile, float angle, Vector3 position)
    {
        if (_dugTileDictionary.ContainsKey(ToVecInt(position)))
        {
            if (_dugTileDictionary[ToVecInt(position)].Item1.Equals(tile.name))
            {
                if (_dugTileDictionary[ToVecInt(position)].Item2 == angle)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        return false;
    }

    private Vector3Int ToVecInt(Vector3 vector)
    {
        return normalTilemap.WorldToCell(vector);
    }

    private bool PlayerAfterOffset()
    {
        Vector3Int currentCell = normalTilemap.WorldToCell(transform.position);
        int dir = 0;
        
        if (_currentDirection.Equals("right") || _currentDirection.Equals("left"))
        {
            dir = _currentDirection.Equals("right") ? -1 : 1;
            return currentCell == normalTilemap.WorldToCell
                (transform.position + new Vector3(dir*playerOffset, 0, 0));
        }
        dir = _currentDirection.Equals("up") ? -1 : 1;
        return currentCell == normalTilemap.WorldToCell
            (transform.position + new Vector3(0, dir * playerOffset, 0));
    }

    private void SameAxisLogic(int i)
    {
        _previousDirection = _currentDirection;
        if (IsAlreadyIn(dugTiles[1], _moveValues[_previousDirection], transform.position) ||
            IsAlreadyIn(dugTiles[1], _moveValues[_previousDirection] + 180 % 360, transform.position))
        {
            return;
        }

        if (!IsAlreadyIn(dugTiles[0], Math.Min(_moveValues[_previousDirection],(_moveValues[_previousDirection]+180)%360), transform.position) && PreviousTileCheck())
        {
            PlaceTile(dugTilemaps[i], dugTiles[0], Math.Min(_moveValues[_previousDirection],(_moveValues[_previousDirection]+180)%360), transform.position);

            return;
        }

        if (!PlayerBeforeTileCenter() && PreviousTileCheck())
        {
            PlaceTile(dugTilemaps[i], dugTiles[1], Math.Min(_moveValues[_previousDirection],(_moveValues[_previousDirection]+180)%360), transform.position);
            _preTile = ToVecInt(transform.position);
        }
    }

    private void OtherAxisLogic(int i)
    {
        float movementValue = (_moveValues[_currentDirection] + _moveValues[_previousDirection]) % 360;
        if (movementValue > 180)
        {
            //need to place tile[2] with 0 angle
            if (_moveValues[_previousDirection] - _moveValues[_currentDirection] < 0)
            {

                if (CloseDistance(transform.position, GetCenterPosOfCell(transform.position), 0.5f) &&
                    !IsAlreadyIn(dugTiles[2], 0, transform.position))
                {
                    PlaceTile(dugTilemaps[i], dugTiles[2], 0, transform.position);
                    _preTile = ToVecInt(transform.position);
                    _previousDirection = _currentDirection;
                }
            }
            //need to place tile[2] with 180 angle
            else if (CloseDistance(transform.position, GetCenterPosOfCell(transform.position), 0.5f) &&
                     !IsAlreadyIn(dugTiles[2], 180, transform.position))
            {
                PlaceTile(dugTilemaps[i], dugTiles[2], 180, transform.position);
                _preTile = ToVecInt(transform.position);
                _previousDirection = _currentDirection;
            }
        }
        else
        {
            //need to place tile[2] with 90 angle
            if (_previousDirection.Equals("right") || _previousDirection.Equals("down"))
            {
                if (CloseDistance(transform.position, GetCenterPosOfCell(transform.position), 0.5f) &&
                    !IsAlreadyIn(dugTiles[2], 90, transform.position))
                {
                    PlaceTile(dugTilemaps[i], dugTiles[2], 90, transform.position);
                    _preTile = ToVecInt(transform.position);
                    _previousDirection = _currentDirection;
                }
            }
            else
            {
                //need to place tile[2] with 270
                if (CloseDistance(transform.position, GetCenterPosOfCell(transform.position), 0.5f) &&
                    !IsAlreadyIn(dugTiles[2], 270, transform.position))
                {
                    PlaceTile(dugTilemaps[i], dugTiles[2], 270, transform.position);
                    _preTile = ToVecInt(transform.position);
                    _previousDirection = _currentDirection;
                }
            }
        }
    }

    private bool PreviousTileCheck()
    {
        return normalTilemap.WorldToCell(PreviousTile()) == _previousTile;
    }

    private Vector3 GetCenterPosOfCell(Vector3 position)
    {
        Vector3Int cellPos = normalTilemap.WorldToCell(position);
        return normalTilemap.GetCellCenterWorld(cellPos);  
    }

    private bool CloseDistance(Vector3 one, Vector3 two, float dist)
    {
        return Math.Abs(Vector3.Distance(one,two)) < dist;
    }

    private void UpdateTileHistory()
    {
        var newTile = dugTilemaps[0].WorldToCell(transform.position);
        if (newTile != _currentTile)
        {
            _previousTile = _currentTile;
            _currentTile = newTile;
        }
    }

    public static Vector3Int GetPreviousTile()
    {
        return _previousTile;
    }
    

}