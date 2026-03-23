using System.Collections.Generic;

public interface IPreviewablePresenter : IPresenter
{
    void ApplyPreviewSignals(PreviewSignal activeSignals, List<PreviewTransition> transitions);
    void ResetPreview();
}
