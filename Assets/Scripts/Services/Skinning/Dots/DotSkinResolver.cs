using UnityEngine;


public class DotSkinResolver : ISkinResolver<Dot>
{
    public Skin ResolveSkin(Dot dot)
    {
        return ResolveDotSkin(dot);
    }
    public Skin ResolveDotSkin(Dot dot)
    {
        var colorScheme = ServiceProvider.Instance.GetService<ColorSchemeService>().CurrentColorScheme;
        if (dot.TryGetModel(out Colorable colorable))
        {
            var color = ServiceProvider.Instance.GetService<ColorSchemeService>().FromDotColor(colorable.Color);
            return new Skin(color, color, color);
        }
        return dot.DotType switch
        {
            DotType.Bomb => new Skin(colorScheme.bomb.baseColor, colorScheme.bomb.accentColor, colorScheme.bomb.detailColor),
            _ => throw new System.Exception("Invalid dot type."),
        };
    
    }
   

    
}