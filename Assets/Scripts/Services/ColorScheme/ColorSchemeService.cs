using System;
using System.Net.NetworkInformation;
using UnityEngine;
using Color = UnityEngine.Color;

public class ColorSchemeService : MonoBehaviour
{
    [SerializeField] private ColorScheme[] _colorSchemes;
    public ColorScheme CurrentColorScheme { get; private set; }


    

    public void Initialize(int index)
    {
        if (index >= 0 && index < _colorSchemes.Length)
        {
            CurrentColorScheme = _colorSchemes[index];
        }
        else
        {
            Debug.LogError("Invalid color scheme index.");
        }
    }


    public Color FromDotColor(DotColor dotColor)
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
    public Color ToDotColor(IBoardEntity entity)
    {
        if (entity.TryGetModel(out Colorable colorable))
        {
            return FromDotColor(colorable.Color);
        }
        else
        {
            return CurrentColorScheme.blank;
        }
    }
    public Color ToDotColor(Dot dot)
    {
        if (dot.TryGetModel(out Colorable colorable))
        {
            return FromDotColor(colorable.Color);
        }
        else
        {
            return CurrentColorScheme.blank;
        }

    }

}

