using System;
using System.Collections.Generic;

/// <summary>
/// Maintains active/inactive state for independent dot effect channels.
/// Channel transitions are edge-triggered so enter/exit callbacks only run when state changes.
/// </summary>
public sealed class DotEffectOrchestrator
{
    private readonly Dictionary<DotEffectChannel, bool> _activeByChannel = new();

    /// <summary>
    /// Sets the active state for a channel and invokes the appropriate transition callback
    /// only when the state changes.
    /// </summary>
    /// <param name="channel">The channel being updated.</param>
    /// <param name="isActive">The desired active state.</param>
    /// <param name="onEnter">Invoked when the channel transitions from inactive to active.</param>
    /// <param name="onExit">Invoked when the channel transitions from active to inactive.</param>
    public void SetChannel(DotEffectChannel channel, bool isActive, Action onEnter, Action onExit)
    {
        _activeByChannel.TryGetValue(channel, out bool wasActive);
        if (wasActive == isActive) return;

        _activeByChannel[channel] = isActive;
        if (isActive) onEnter?.Invoke();
        else onExit?.Invoke();
    }

    /// <summary>
    /// Returns true when the requested channel is currently active.
    /// </summary>
    /// <param name="channel">The channel to query.</param>
    public bool IsActive(DotEffectChannel channel)
    {
        return _activeByChannel.TryGetValue(channel, out bool active) && active;
    }

    /// <summary>
    /// Exits all active channels and clears orchestration state.
    /// </summary>
    /// <param name="onExit">
    /// Callback invoked once for each channel that was active prior to reset.
    /// </param>
    public void Reset(Action<DotEffectChannel> onExit)
    {
        foreach (var pair in _activeByChannel)
        {
            if (!pair.Value) continue;
            onExit?.Invoke(pair.Key);
        }
        _activeByChannel.Clear();
    }
}
