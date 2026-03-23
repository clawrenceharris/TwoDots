using System;
using System.Collections.Generic;

/// <summary>
/// Transition-only bridge for preview audio hooks.
/// Audio systems can subscribe and map transitions to SFX without replay spam.
/// </summary>
public sealed class PreviewAudioBridge
{
    public event Action<PreviewTransition> OnPreviewTransition;

    public void Publish(List<PreviewTransition> transitions)
    {
        if (transitions == null || transitions.Count == 0) return;
        foreach (var transition in transitions)
        {
            OnPreviewTransition?.Invoke(transition);
        }
    }
}
