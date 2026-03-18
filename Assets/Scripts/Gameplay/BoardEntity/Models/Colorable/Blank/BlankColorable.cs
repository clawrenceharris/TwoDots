using UnityEngine;

public class BlankColorableDot : Colorable
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