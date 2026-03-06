using UnityEngine;

public class BlankColorableDot : ColorableDot
{
    public BlankColorableDot(Dot dot) : base(dot, false)
    {
    }
       

    public override bool Equals(DotColor colorable)
    {

        return true;
    }

    public override DotColor GetComparableColor(DotColor color)
    {
        return color;
    }
}