using UnityEngine;

public interface ISkinResolver
{
    DotSkin ResolveDotSkin(Dot dot);
}

public class SkinResolver : ISkinResolver
{
    public DotSkin ResolveDotSkin(Dot dot)
    {
        Color baseColor = ResolveBaseColor(dot);
        Color accentColor = ResolveAccentColor(dot);
        Color detailColor = ResolveDetailColor(dot);
        return new DotSkin(baseColor, accentColor, detailColor);
    }

    private static Color ResolveDetailColor(Dot dot)
    {
         if (dot.TryGetModel(out ColorableDot colorable))
            return ResolveDotColor(colorable.Color);
        if (ColorSchemeService.GetDotColorScheme(dot.DotType) is { } colorScheme)
        {
            return colorScheme.detailColor;
        }
        return Color.white;
    }
    private static Color ResolveAccentColor(Dot dot)
    {
         if (dot.TryGetModel(out ColorableDot colorable))
            return ResolveDotColor(colorable.Color);
        if (ColorSchemeService.GetDotColorScheme(dot.DotType) is { } colorScheme)
        {
            return colorScheme.accentColor;
        }
        return Color.white;
    }
    private static Color ResolveBaseColor(Dot dot)
    {
        if (dot.TryGetModel(out ColorableDot colorable))
        {
            return ResolveDotColor(colorable.Color);
        }

        if (ColorSchemeService.GetDotColorScheme(dot.DotType) is { } colorScheme)
        {
            return colorScheme.baseColor;
        }
        return Color.white;
    }

    private static Color ResolveDotColor(DotColor dotColor)
    {
        return ColorSchemeService.CurrentColorScheme == null
            ? Color.white
            : ColorSchemeService.FromDotColor(dotColor);
    }
   

    
}