using System.Collections.Generic;
using UnityEngine;

public class CascadeContext
{
    public IBoardPresenter Board { get; }
    public int TurnIndex { get; }
    public int ChainIndex { get; set; }

    public ConnectionCompletedPayload ConnectionPayload { get; private set; }
    public bool ConnectionPayloadConsumed { get; private set; }

    public IReadOnlyList<string> RecentClearedDotIds => _recentClearedDotIds;
    public IReadOnlyList<Vector2Int> RecentClearedPositions => _recentClearedPositions;

    public HashSet<string> ClearedDotIds { get; } = new();

    private readonly List<string> _recentClearedDotIds = new();
    private readonly List<Vector2Int> _recentClearedPositions = new();

    public CascadeContext(IBoardPresenter board, ConnectionCompletedPayload payload, int turnIndex = 0)
    {
        Board = board;
        ConnectionPayload = payload;
        TurnIndex = turnIndex;
    }

    public bool TryConsumeConnectionPayload(out ConnectionCompletedPayload payload)
    {
        payload = ConnectionPayload;
        if (payload == null || ConnectionPayloadConsumed) return false;
        ConnectionPayloadConsumed = true;
        return true;
    }

    public void SetRecentClears(IEnumerable<string> dotIds, IEnumerable<Vector2Int> positions)
    {
        _recentClearedDotIds.Clear();
        _recentClearedPositions.Clear();
        if (dotIds != null)
            _recentClearedDotIds.AddRange(dotIds);
        if (positions != null)
            _recentClearedPositions.AddRange(positions);
    }

    public void ClearRecentClears()
    {
        _recentClearedDotIds.Clear();
        _recentClearedPositions.Clear();
    }
}
