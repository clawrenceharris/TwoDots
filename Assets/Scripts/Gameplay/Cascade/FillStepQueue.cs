using System.Collections.Generic;

/// <summary>
/// Priority queue for fill steps. Dequeue returns the highest-priority step (then lowest sequence).
/// Used by CascadeRunner for pre-gravity and post-fill phases.
/// </summary>
public class FillStepQueue
{
    private readonly List<FillStep> _items = new();

    /// <summary>Number of steps currently in the queue.</summary>
    public int Count => _items.Count;

    /// <summary>Adds a step and assigns the next sequence number (for tie-breaking).</summary>
    public void Enqueue(FillStep step, ref int sequence)
    {
        if (step == null) return;
        step.Sequence = sequence++;
        _items.Add(step);
    }

    /// <summary>Removes and returns the highest-priority step, or false if empty.</summary>
    public bool TryDequeue(out FillStep step)
    {
        if (_items.Count == 0)
        {
            step = null;
            return false;
        }

        _items.Sort(CompareSteps);
        step = _items[0];
        _items.RemoveAt(0);
        return true;
    }

    /// <summary>Removes all steps from the queue.</summary>
    public void Clear()
    {
        _items.Clear();
    }

    private static int CompareSteps(FillStep a, FillStep b)
    {
        int priority = b.Priority.CompareTo(a.Priority);
        if (priority != 0) return priority;
        return a.Sequence.CompareTo(b.Sequence);
    }
}
