public interface IPreviewablePresenter : IPresenter
{
    void PreviewHit();
    void PreviewClear();
    void StopPreview();
}