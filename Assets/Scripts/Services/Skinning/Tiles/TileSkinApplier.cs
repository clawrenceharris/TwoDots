
public class TileSkinApplier : ISkinApplier<TileView>
{
    public void Apply(TileView view, Skin skin)
    {
        if (view == null || view.Renderer == null) return;
        view.Renderer.ApplySkin(skin);
    }
}
