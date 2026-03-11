using UnityEngine;

public class Tile : BoardEntity
{
    private readonly TileType _tileType;
    public TileType TileType => _tileType;
 
    public Tile(TileType type, Vector2Int position) : base(position)
    {
        _tileType = type;
       
    }
}