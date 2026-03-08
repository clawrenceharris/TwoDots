using UnityEngine;


public class TileSkinResolver : ISkinResolver<Tile>
{
    public Skin ResolveSkin(Tile tile)
    {
        Color baseColor = ResolveBaseColor(tile);
        Color accentColor = ResolveAccentColor(tile);
        Color detailColor = ResolveDetailColor(tile);
        return new Skin(baseColor, accentColor, detailColor);
    }
   
    private static Color ResolveDetailColor(Tile tile)
    {
        
        if (ColorSchemeService.GetTileColorScheme(tile.TileType) is { } colorScheme)
        {
            return colorScheme.detailColor;
        }
        return Color.white;
    }
    private static Color ResolveAccentColor(Tile tile)
    {
        
        if (ColorSchemeService.GetTileColorScheme(tile.TileType) is { } colorScheme)
        {
            return colorScheme.accentColor;
        }
        return Color.white;
    }
    private static Color ResolveBaseColor(Tile tile)
    {
       

        if (ColorSchemeService.GetTileColorScheme(tile.TileType) is { } colorScheme)
        {
            return colorScheme.baseColor;
        }
        return Color.white;
    }

    
}