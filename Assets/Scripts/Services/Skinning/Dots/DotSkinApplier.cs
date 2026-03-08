
public class DotSkinApplier : ISkinApplier<DotView>
{
    public void Apply(DotView view, Skin skin)
    {
        if (view == null || view.Renderer == null) return;
        view.Renderer.ApplySkin(skin);
    }
}
