public class BeetlePreviewRule : IPreviewRule
{
    private readonly PreviewConfig _config;

    public BeetlePreviewRule(PreviewConfig config = null)
    {
        _config = config;
    }

    public bool CanEvaluate(EntityPresenter presenter)
    {
        if (_config != null && !_config.EnableBeetlePreview) return false;
        return presenter?.Entity is Dot dot && dot.DotType == DotType.Beetle;
    }

    public PreviewSignal Evaluate(EntityPresenter presenter, PreviewContext context)
    {
        if (presenter?.Entity is not Dot dot || context == null) return PreviewSignal.None;

        PreviewSignal signals = PreviewSignal.None;
        if (context.HasOneHitLeft(dot))
        {
            signals |= PreviewSignal.OneHitLeft | PreviewSignal.Shake;
        }

        if (context.IsInActivePath(dot.ID))
        {
            signals |= PreviewSignal.ConnectedActive | PreviewSignal.WingFlap;
        }

        return signals;
    }
}
