using System.Collections.Generic;

public class FillStepQueue
{
    private readonly List<FillStep> _items = new();

    public int Count => _items.Count;

    public void Enqueue(FillStep step, ref int sequence)
    {
        if (step == null) return;
        step.Sequence = sequence++;
        _items.Add(step);
    }

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
