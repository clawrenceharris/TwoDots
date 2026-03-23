public interface IPreviewRule
{
    bool CanEvaluate(EntityPresenter presenter);
    PreviewSignal Evaluate(EntityPresenter presenter, PreviewContext context);
}
