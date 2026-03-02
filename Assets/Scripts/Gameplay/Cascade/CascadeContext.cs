using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Read-only (plus recent-clears) context for one cascade run. Holds the board, connection payload,
/// turn/chain indices, and the set of recently cleared positions/IDs so producers can react (e.g.
/// seeds adjacent to cleared cells, gems next to cleared cells).
/// </summary>
public class CascadeContext
{
    /// <summary>Board to query and mutate via the runner.</summary>
    public IBoardPresenter Board { get; }
    /// <summary>Turn index for this cascade (e.g. move number).</summary>
    public int TurnIndex { get; }
    /// <summary>Current cascade cycle index; increments each loop until stable.</summary>
    public int ChainIndex { get; set; }

    /// <summary>Payload from the connection that started this cascade; may be null.</summary>
    public ConnectionContext ConnectionContext { get; private set; }
    /// <summary>True after TryConsumeConnectionPayload has been called successfully once.</summary>
    public bool ConnectionPayloadConsumed { get; private set; }

    /// <summary>Dot IDs cleared by the most recently executed step(s).</summary>
    public IReadOnlyList<string> RecentClearedDotIds => _recentClearedDotIds;
    /// <summary>Grid positions cleared by the most recent step(s); used by adjacency producers.</summary>
    public IReadOnlyList<Vector2Int> RecentClearedPositions => _recentClearedPositions;

    /// <summary>All dot IDs cleared during this cascade (cumulative).</summary>
    public HashSet<string> ClearedDotIds { get; } = new();

    private readonly List<string> _recentClearedDotIds = new();
    private readonly List<Vector2Int> _recentClearedPositions = new();

    /// <summary>Creates context for a cascade run with the given board and optional connection payload.</summary>
    public CascadeContext(IBoardPresenter board, ConnectionContext payload, int turnIndex = 0)
    {
        Board = board;
        ConnectionContext = payload;
        TurnIndex = turnIndex;
    }

    /// <summary>Returns the connection payload and marks it consumed; returns false if already consumed or null.</summary>
    public bool TryConsumeConnectionPayload(out ConnectionContext payload)
    {
        payload = ConnectionContext;
        if (payload == null || ConnectionPayloadConsumed) return false;
        ConnectionPayloadConsumed = true;
        return true;
    }

    /// <summary>Sets the recent-cleared IDs and positions (e.g. after executing a step).</summary>
    public void SetRecentClears(IEnumerable<string> dotIds, IEnumerable<Vector2Int> positions)
    {
        _recentClearedDotIds.Clear();
        _recentClearedPositions.Clear();
        if (dotIds != null)
            _recentClearedDotIds.AddRange(dotIds);
        if (positions != null)
            _recentClearedPositions.AddRange(positions);
    }

    /// <summary>Clears the recent-clears lists (e.g. when finishing a phase queue).</summary>
    public void ClearRecentClears()
    {
        _recentClearedDotIds.Clear();
        _recentClearedPositions.Clear();
    }
}
