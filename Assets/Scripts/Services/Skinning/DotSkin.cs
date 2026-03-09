using UnityEngine;

public readonly struct DotSkin
{
    public Color BaseColor { get; }
    public Color AccentColor { get; }
    public Color DetailColor { get; }

    public DotSkin(Color baseColor, Color accentColor, Color detailColor)
    {
        BaseColor = baseColor;
        AccentColor = accentColor;
        DetailColor = detailColor;
    }
}
