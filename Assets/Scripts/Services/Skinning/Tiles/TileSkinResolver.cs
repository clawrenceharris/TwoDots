using UnityEngine;


public class TileSkinResolver : ISkinResolver<Tile>
{
    public Skin ResolveSkin(Tile tile)
    {

        return ResolveTileSkin(tile);
    }
    
    public static Skin ResolveTileSkin(Tile tile)
    {
        var colorScheme = ServiceProvider.Instance.GetService<ColorSchemeService>().CurrentColorScheme;
        return tile.TileType switch
        {
            TileType.Block or
            TileType.OneSidedBlock or
            TileType.EmptyTile => new Skin(colorScheme.backgroundColor, colorScheme.backgroundColor, colorScheme.backgroundColor),
            _ => throw new System.Exception("Invalid tile type."),
        };
    }
   
    
}