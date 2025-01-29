using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Managers
{
    public class TileManager : MonoBehaviour
    {
        private enum TileNames
        {
            HalfTile,
            FullTile,
            CornerTile,
            Black,
            HalfFull,
            HalfFullFlip
        }

        [SerializeField] TileColliderManager tileColliderManager;
        [SerializeField] private MonoBehaviour player;
        [SerializeField] private Tilemap dugTilemap;
        [SerializeField] private List<TileBase> dugTiles;
        private readonly int _upperTileBound = 2;
        private readonly Dictionary<Vector3Int, (int, int)> _activeTiles = new();
        private readonly Dictionary<string, int> _moveValues = new()
        {
            { "up", 0 },
            { "right", 90 },
            { "down", 180 },
            { "left", 270 },
        };

        private void Start()
        {
            AddStartTiles();
        }

        public void SetTile(Vector3 position, string preDirection, string currDirection, bool beforeCenter)
        {
            Vector3Int cellPos = ToTilePos(position);
            (int, int) newTile; // first int is the index in dugTiles list and second int is the angle of the tile
            if (!_activeTiles.ContainsKey(cellPos) && cellPos.y < _upperTileBound) //if no tile in position
            {
                ScoreManager.Instance.AddScore(10);
                newTile = MatchTile(preDirection, currDirection, beforeCenter);
                PlaceTile(cellPos, newTile);
                return;
            }

            if (cellPos.y < _upperTileBound)
            {
                var existingTile = _activeTiles[cellPos];
                if (existingTile.Item1 == (int)TileNames.Black) // if "black" tile in position
                    return;
                newTile = MatchTile(preDirection, currDirection, beforeCenter);
                if (newTile == existingTile) // if same tile 
                    return;
                if (existingTile.Item1 == (int)TileNames.HalfTile)
                {
                    newTile = HalfLogic(existingTile, newTile);
                    PlaceTile(cellPos, newTile);
                    return;
                }

                if (existingTile.Item1 == (int)TileNames.FullTile)
                {
                    newTile = FullLogic(existingTile, newTile);
                    PlaceTile(cellPos, newTile);
                    return;
                }

                if (existingTile.Item1 == (int)TileNames.CornerTile)
                {
                    if (newTile.Item1 == (int)TileNames.HalfTile || newTile.Item1 == (int)TileNames.FullTile)
                    {
                        Vector3 centerPos = dugTilemap.GetCellCenterLocal(cellPos);
                        if (existingTile.Item2 == 0)
                        {
                            if (position.x > centerPos.x)
                            {
                                PlaceTile(cellPos, MatchHalfFull("down"));
                                return;
                            }
                            if (position.y > centerPos.y)
                            {
                                PlaceTile(cellPos, MatchHalfFull("left"));
                                return;
                            }
                        }

                        if (existingTile.Item2 == 90)
                        {
                            if (position.x > centerPos.x)
                            {
                                PlaceTile(cellPos, MatchHalfFull("up"));
                                return;
                            }

                            if (position.y < centerPos.y)
                            {
                                PlaceTile(cellPos, MatchHalfFull("left"));
                                return;
                            }
                        }

                        if (existingTile.Item2 == 180)
                        {
                            if (position.x < centerPos.x)
                            {
                                PlaceTile(cellPos, MatchHalfFull("up"));
                                return;
                            }

                            if (position.y < centerPos.y)
                            {
                                PlaceTile(cellPos, MatchHalfFull("right"));
                                return;
                            }
                        }

                        if (existingTile.Item2 == 270)
                        {
                            if (position.x < centerPos.x)
                            {
                                PlaceTile(cellPos, MatchHalfFull("down"));
                                return;
                            }

                            if (position.y > centerPos.y)
                            {
                                PlaceTile(cellPos, MatchHalfFull("right"));
                                return;
                            }
                        }
                    }
                    else if (newTile.Item1 == (int)TileNames.FullTile)
                    {

                    }

                    newTile = CornerLogic(existingTile, newTile);
                    PlaceTile(cellPos, newTile);
                    return;
                }

                if (existingTile.Item1 == (int)TileNames.HalfFull)
                {
                    if (newTile.Item1 == 1)
                    {
                        if (existingTile.Item2 == 0)
                        {
                            if (position.x < dugTilemap.GetCellCenterLocal(cellPos).x)
                            {
                                PlaceTile(cellPos, ((int)TileNames.Black, 0));
                                return;
                            }

                            PlaceTile(cellPos, existingTile);
                            return;
                        }
                        else
                        {
                            if (position.y > dugTilemap.GetCellCenterLocal(cellPos).y)
                            {
                                PlaceTile(cellPos, ((int)TileNames.Black, 0));
                                return;
                            }

                            PlaceTile(cellPos, existingTile);
                            return;
                        }
                    }

                    newTile = HalfFullLogic(existingTile, newTile);
                    PlaceTile(cellPos, newTile);
                    return;
                }

                if (existingTile.Item1 == (int)TileNames.HalfFullFlip)
                {
                    if (newTile.Item1 == 1)
                    {
                        if (existingTile.Item2 == 0)
                        {
                            if (position.x > dugTilemap.GetCellCenterLocal(cellPos).x)
                            {
                                PlaceTile(cellPos, ((int)TileNames.Black, 0));
                                return;
                            }

                            PlaceTile(cellPos, existingTile);
                            return;
                        }
                        else
                        {
                            if (position.y < dugTilemap.GetCellCenterLocal(cellPos).y)
                            {
                                PlaceTile(cellPos, ((int)TileNames.Black, 0));
                                return;
                            }

                            PlaceTile(cellPos, existingTile);
                            return;
                        }
                    }

                    newTile = HalfFullFlipLogic(existingTile, newTile);
                    PlaceTile(cellPos, newTile);
                }
            }

        }

        private Vector3Int ToTilePos(Vector3 pos)
        {
            return dugTilemap.WorldToCell(pos);
        }

        private void UpdateActiveTiles(Vector3Int cellPos, (int, int)tile)
        {
            _activeTiles[cellPos] = tile;
        }

        private void PlaceTile(Vector3Int cellPos, (int, int) tile)
        {
            if (!_activeTiles.ContainsKey(cellPos) || _activeTiles[cellPos] != tile)
            {
                dugTilemap.SetTile(cellPos, dugTiles[tile.Item1]);
                var rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, -tile.Item2));
                dugTilemap.SetTransformMatrix(cellPos, rotationMatrix);
                dugTilemap.RefreshTile(cellPos);
                UpdateActiveTiles(cellPos, tile);
                EventManager.UpdateDugTile?.Invoke(cellPos);
            }
        }

        private (int, int) MatchTile(string preDirection, string currDirection, bool beforeCenter)
        {
            int angle;
            if (_moveValues[currDirection] == _moveValues[preDirection] ||
                _moveValues[currDirection] == (_moveValues[preDirection] + 180) % 360) 
            {
                int tileNumber = beforeCenter ? (int)TileNames.HalfTile : (int)TileNames.FullTile;
                if (tileNumber == (int)TileNames.HalfTile)
                {
                    return (tileNumber, _moveValues[currDirection]);
                }
                return (tileNumber, Mathf.Min(_moveValues[preDirection], (_moveValues[preDirection] + 180) % 360));
            }
            if (_moveValues[currDirection] + _moveValues[preDirection] == 270)
            {
                angle = preDirection.Equals("up") || preDirection.Equals("right") ? 0 : 180;
                return ((int)TileNames.CornerTile, angle);
            }
            angle = preDirection.Equals("up") || preDirection.Equals("left") ? 270 : 90;
            return ((int)TileNames.CornerTile, angle);
        }

    
        private (int, int) HalfLogic((int, int) existingTile, (int, int) newTile)
        {
            if (newTile.Item1 == existingTile.Item1) // if also want to put half 
            {
                //if I'm coming from the current tile 
                if (PlayerTileMaker.GetPreviousTile() == GetAdjacentTile(GetKeyByValue((existingTile.Item2 + 180) % 360)))
                {
                    return existingTile;
                }
                //if this is opposite tile 
                if (existingTile.Item2 == (newTile.Item2 + 180) % 360)
                {
                    return ((int)TileNames.FullTile, Math.Min(existingTile.Item2,(existingTile.Item2 + 180)%360));
                }
                //put corner because it's tile from other direction

                return MatchTile(GetKeyByValue(newTile.Item2),
                    GetKeyByValue((existingTile.Item2 + 180) %360), true);
            }

            if (newTile.Item1 == (int)TileNames.FullTile)
            {
                if (newTile.Item2 == (existingTile.Item2 + 180) % 360
                    || newTile.Item2 == existingTile.Item2)
                {
                    return newTile;
                }
                if (PlayerTileMaker.GetPreviousTile() == GetAdjacentTile(GetKeyByValue((existingTile.Item2 + 180) % 360)))
                {
                    return existingTile;
                }
                return MatchHalfFull( GetKeyByValue(existingTile.Item2));
            }
            // want to put corner
            if (newTile.Item2 == existingTile.Item2 || newTile.Item2 == (existingTile.Item2 +360 - 90) % 360)
            {
                return newTile;
            }
            return MatchHalfFull( GetKeyByValue(existingTile.Item2) + 90);
        }
        private (int, int) FullLogic((int, int) existingTile, (int, int) newTile)
        {
            if (newTile.Item1 == existingTile.Item1)
            {
                if ((newTile.Item2 + 90) % 360 == existingTile.Item2 || (newTile.Item2 + 180) % 360 == existingTile.Item2)
                {
                    return ((int)TileNames.Black, 0);
                }
                return existingTile;
            }

            if (newTile.Item1 == (int)TileNames.HalfTile)
            {
                if (newTile.Item2 == (existingTile.Item2 + 180) % 360
                    || newTile.Item2 == existingTile.Item2)
                {
                    return existingTile;
                }
                return MatchHalfFull( GetKeyByValue((newTile.Item2+180)%360));
            }
            //corner
            int entryDirection;
            if (existingTile.Item2 == 0)
            {
                entryDirection = newTile.Item2 == 0 || newTile.Item2 == 90 ? 270 : 90;
            }
            else
            {
                entryDirection = newTile.Item2 == 0 || newTile.Item2 == 270 ? 180 : 0;
            }
            return MatchHalfFull(GetKeyByValue(entryDirection)); 
        }

        private (int, int) CornerLogic((int, int) existingTile, (int, int) newTile)
        {
            if (newTile.Item1 == existingTile.Item1)
            {
                if (existingTile.Item2 == (newTile.Item2 + 180) % 360)
                {
                    return ((int)TileNames.Black, 0);
                }
                return MatchHalfFull(GetKeyByValue((existingTile.Item2 + 270) % 360));
            }

            if (newTile.Item1 == (int)TileNames.HalfTile)
            {
                if (newTile.Item2 == existingTile.Item2 || newTile.Item2 == (existingTile.Item2 +90) % 360)
                {
                    return existingTile;
                }
                if ((newTile.Item2 + 270) % 360 == existingTile.Item2)
                {
                    return MatchHalfFull(GetKeyByValue((existingTile.Item2 + 270) % 360));
                }
                else
                {
                    return MatchHalfFull(GetKeyByValue((existingTile.Item2 + 180) % 360));
                }
            }
            return existingTile;
        }

        private (int,int) HalfFullLogic((int, int) existingTile, (int, int) newTile)
        {
            if (newTile.Item1 == existingTile.Item1)
            {
                return ((int)TileNames.Black, 0);
            }
        
            if (newTile.Item1 == (int)TileNames.HalfTile)
            {
                if (newTile.Item2 == existingTile.Item2 + 90 &&
                    PlayerTileMaker.GetPreviousTile() == GetAdjacentTile(GetKeyByValue((newTile.Item2 + 180) % 360)))
                {
                    return ((int)TileNames.Black, 0);
                }
                return existingTile;
            }
            return existingTile;
        }
        private (int, int) HalfFullFlipLogic((int, int) existingTile, (int, int) newTile)
        {
            if (newTile.Item1 == existingTile.Item1)
            {
                return ((int)TileNames.Black, 0);
            }

            if (newTile.Item1 == (int)TileNames.HalfTile)
            {
                if (newTile.Item2 == (existingTile.Item2 + 270) % 360
                    && PlayerTileMaker.GetPreviousTile() == GetAdjacentTile(GetKeyByValue((newTile.Item2+180)%360)))
                {
                    return ((int)TileNames.Black, 0);
                }
                return existingTile;
            }
            return existingTile;
        }

        private string GetKeyByValue(int value)
        {
            foreach (var kvp in _moveValues)
            {
                if (kvp.Value == value)
                {
                    return kvp.Key; // Return the key
                }
            }
            return null;
        }

        private (int, int) MatchHalfFull(string entryDirection)
        {
            int angle;
            if (entryDirection.Equals("right") || entryDirection.Equals("down"))
            {
                angle = entryDirection.Equals("right") ? 0 : 90;
                return ((int)TileNames.HalfFull, angle);
            }
            else
            {
                angle = entryDirection.Equals("left") ? 0 : 90;
                return ((int)TileNames.HalfFullFlip, angle);
            }
        }

        private Vector3Int GetAdjacentTile(string direction)
        {
            switch (direction)
            {
                case "right":
                    return dugTilemap.WorldToCell(player.transform.position) + new Vector3Int(1, 0, 0);
                case "left":
                    return dugTilemap.WorldToCell(player.transform.position) + new Vector3Int(-1, 0, 0);
                case "up":
                    return dugTilemap.WorldToCell(player.transform.position) + new Vector3Int(0, 1, 0);
                
            }
            return dugTilemap.WorldToCell(player.transform.position) + new Vector3Int(0, -1, 0);
        }

        private void AddStartTiles()
        {
            _activeTiles[new Vector3Int(3, 0, 0)] = (0, 270);
            _activeTiles[new Vector3Int(4, 0, 0)] = (1, 90);
            _activeTiles[new Vector3Int(5, 0, 0)] = (1, 90);
            _activeTiles[new Vector3Int(6, 0, 0)] = (0, 90);
            _activeTiles[new Vector3Int(-4, 0, 0)] = (0, 0);
            _activeTiles[new Vector3Int(-4, -1, 0)] = (1, 0);
            _activeTiles[new Vector3Int(-4, -2, 0)] = (1, 0);
            _activeTiles[new Vector3Int(-4, -3, 0)] = (1, 0);
            _activeTiles[new Vector3Int(-4, -4, 0)] = (0, 180);
            _activeTiles[new Vector3Int(-1, -5, 0)] = (0, 270);
            _activeTiles[new Vector3Int(0, -5, 0)] = (1, 90);
            _activeTiles[new Vector3Int(1, -5, 0)] = (0, 90);
            _activeTiles[new Vector3Int(0, -8, 0)] = (0, 90);
            _activeTiles[new Vector3Int(-1, -8, 0)] = (1, 90);
            _activeTiles[new Vector3Int(-2, -8, 0)] = (1, 90);
            _activeTiles[new Vector3Int(-3, -8, 0)] = (0, 270);
            _activeTiles[new Vector3Int(3, -7, 0)] = (0, 0);
            _activeTiles[new Vector3Int(3, -8, 0)] = (1, 0);
            _activeTiles[new Vector3Int(3, -9, 0)] = (1, 0);
            _activeTiles[new Vector3Int(3, -10, 0)] = (1, 0);
            _activeTiles[new Vector3Int(3, -11, 0)] = (0, 180);
        


        }

        public Dictionary<Vector3Int, (int, int)> GetActiveTiles()
        {
            return _activeTiles;
        }
    }
}
