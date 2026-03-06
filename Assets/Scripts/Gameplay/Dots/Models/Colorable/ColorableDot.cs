using System;

public class ColorableDot : DotModel, IColorableModel, IEquatable<DotColor>
{
    private readonly bool _isColorMutable;
    protected DotColor _color;
    public DotColor Color
    {
        get => _color;
        set
        {
            if (!_isColorMutable) return;
            _color = value;
        }
    }
    public ColorableDot(Dot dot, bool isColorMutable = true) : base(dot)
    {
        _isColorMutable = isColorMutable;
    }

    public virtual bool Equals(DotColor other)
    {
        return Color == other;
    }
    public virtual DotColor GetComparableColor(DotColor color)
    {
        return _color;
    }
   
}