using UnityEngine;

public interface IDotSkinApplier
{
    void Apply(DotView view, DotSkin skin);
}

public class DotSkinApplier : IDotSkinApplier
{
    public void Apply(DotView view, DotSkin skin)
    {
        if (view == null || view.DotRenderer == null) return;
        view.DotRenderer.ApplySkin(skin);
    }
}
