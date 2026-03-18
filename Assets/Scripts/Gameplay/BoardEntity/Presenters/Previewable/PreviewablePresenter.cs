public class PreviewablePresenter : EntityPresenter, IPreviewablePresenter
{
    public PreviewablePresenter(BoardEntity entity, EntityView view) : base(entity, view)
    {
    }

    public void PreviewHit()
    {
        throw new System.NotImplementedException();
    }
    public void PreviewClear()
    {
        throw new System.NotImplementedException();
    }
    public void StopPreview()
    {
        throw new System.NotImplementedException();
    }
}