using System;
using System.Net.NetworkInformation;
using UnityEngine;
using Color = UnityEngine.Color;

public class ColorSchemeManager
{
    private ColorScheme[] colorSchemes;

    public static ColorScheme CurrentColorScheme { get; private set; }

    public ColorSchemeManager(ColorScheme[] colorSchemes, int initialIndex)
    {
        this.colorSchemes = colorSchemes;
        SetColorScheme(initialIndex);
    }

    public void SetColorScheme(int index)
    {
        if (index >= 0 && index < colorSchemes.Length)
        {
            CurrentColorScheme = colorSchemes[index];
        }
        else
        {
            Debug.LogError("Invalid color scheme index.");
        }
    }

    public static DotColorScheme GetDotColorScheme(DotType type)
    {
        return type switch
        {
            DotType.Anchor => CurrentColorScheme.anchor,
            DotType.Clock => CurrentColorScheme.clock,
            DotType.Bomb => CurrentColorScheme.bomb,
            DotType.Nesting => CurrentColorScheme.nesting,
            DotType.Beetle => CurrentColorScheme.beetle,
            DotType.Lotus => CurrentColorScheme.lotus,
            _ => throw new ArgumentException("Invalid dot type."),
        };
    }
    public static Color FromDotColor(DotColor dotColor)
    {
        return dotColor switch
        {
            DotColor.Red => CurrentColorScheme.red,
            DotColor.Yellow => CurrentColorScheme.yellow,
            DotColor.Green => CurrentColorScheme.green,
            DotColor.Blue => CurrentColorScheme.blue,
            DotColor.Purple => CurrentColorScheme.purple,


            _ => CurrentColorScheme.blank,
        };
    }
    public static Color ToDotColor(Dot dot)
    {
        if (dot.TryGetComponent(out ColorableModel colorable))
        {
            return FromDotColor(colorable.Color);
        }
        else
        {
            return CurrentColorScheme.blank;
        }
       
    }

}

