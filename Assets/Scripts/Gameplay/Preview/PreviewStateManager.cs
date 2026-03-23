using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class PreviewStateManager
{
    private readonly IBoardPresenter _board;
    private readonly Connection _connection;
    private readonly PreviewConfig _config;
    private readonly List<IPreviewRule> _rules = new();
    private readonly Dictionary<string, PreviewSignal> _activeSignalsByEntityId = new();
    private readonly PreviewAudioBridge _audioBridge = new();

    public PreviewAudioBridge AudioBridge => _audioBridge;

    public PreviewStateManager(IBoardPresenter board, Connection connection, PreviewConfig config = null)
    {
        _board = board;
        _connection = connection;
        _config = config;
    }

    public void RegisterRule(IPreviewRule rule)
    {
        if (rule == null) return;
        _rules.Add(rule);
    }

    public void Recompute()
    {
        if (_board == null) return;
        if (_config != null && !_config.EnablePreview)
        {
            ResetAllPreview();
            return;
        }

        var path = _connection?.Path ?? new List<string>();
        var pathSet = new HashSet<string>(path);
        var context = new PreviewContext(_board, _connection, pathSet, path);

        var allPreviewables = CollectPreviewables();
        var nextSignals = new Dictionary<string, PreviewSignal>(allPreviewables.Count);
        var transitions = new List<PreviewTransition>();

        foreach (var previewable in allPreviewables)
        {
            if (previewable?.Entity == null) continue;

            PreviewSignal mergedSignals = PreviewSignal.None;
            var presenter = previewable as EntityPresenter;
            if (presenter == null) continue;

            foreach (var rule in _rules)
            {
                if (rule == null || !rule.CanEvaluate(presenter)) continue;
                mergedSignals |= rule.Evaluate(presenter, context);
            }

            nextSignals[previewable.Entity.ID] = mergedSignals;
            _activeSignalsByEntityId.TryGetValue(previewable.Entity.ID, out var previousSignals);
            transitions.AddRange(PreviewDiffEngine.BuildTransitions(previewable.Entity.ID, previousSignals, mergedSignals));
        }

        foreach (var previewable in allPreviewables)
        {
            if (previewable?.Entity == null) continue;
            nextSignals.TryGetValue(previewable.Entity.ID, out var activeSignals);
            var relevantTransitions = transitions.Where(t => t.EntityId == previewable.Entity.ID).ToList();
            previewable.ApplyPreviewSignals(activeSignals, relevantTransitions);
        }

        _activeSignalsByEntityId.Clear();
        foreach (var pair in nextSignals)
        {
            _activeSignalsByEntityId[pair.Key] = pair.Value;
        }

        if (_config == null || _config.EnableAudioBridge)
        {
            _audioBridge.Publish(transitions);
        }
    }

    public void ResetAllPreview()
    {
        var previewables = CollectPreviewables();
        foreach (var previewable in previewables)
        {
            previewable.ResetPreview();
        }
        _activeSignalsByEntityId.Clear();
    }

    private List<IPreviewablePresenter> CollectPreviewables()
    {
        var result = new List<IPreviewablePresenter>();
        var dots = _board.GetDotsOnBoard();
        foreach (var dot in dots)
        {
            if (dot == null) continue;
            if (dot.TryGetPresenter(out IPreviewablePresenter previewable))
            {
                result.Add(previewable);
            }
        }
        return result;
    }
}
