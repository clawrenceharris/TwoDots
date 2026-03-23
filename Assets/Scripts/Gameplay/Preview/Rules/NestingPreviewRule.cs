public class NestingPreviewRule : IPreviewRule
{
    private readonly PreviewConfig _config;

    public NestingPreviewRule(PreviewConfig config = null)
    {
        _config = config;
    }

    public bool CanEvaluate(EntityPresenter presenter)
    {
        if (_config != null && !_config.EnableNestingPreview) return false;
        return presenter?.Entity is Dot dot && dot.DotType == DotType.Nesting;
    }

    public PreviewSignal Evaluate(EntityPresenter presenter, PreviewContext context)
    {
        if (presenter?.Entity is not Dot dot) return PreviewSignal.None;
        if (context == null) return PreviewSignal.None;
        if (!context.HasOneHitLeft(dot)) return PreviewSignal.None;
        return PreviewSignal.OneHitLeft | PreviewSignal.Shake | PreviewSignal.Pulse;
    }
}
