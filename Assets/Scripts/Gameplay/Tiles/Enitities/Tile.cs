using UnityEngine;
/// <summary>
/// A tile entity on the board
/// </summary>
public class Tile : BoardEntity
{
    /// <summary> The type of tile </summary>
    private readonly TileType _tileType;
    /// <summary> The type of tile </summary>
    public TileType TileType => _tileType;
    /// <summary>
    /// Initializes a new instance of the <see cref="Tile"/> class.
    /// </summary>
    /// <param name="type">The type of tile</param>
    /// <param name="position">The position of the tile on the board</param>
    public Tile(TileType type, Vector2Int position) : base(position)
    {
        _tileType = type;
       
    }
}