using System.Collections.Generic;

public static class PreviewDiffEngine
{
    public static List<PreviewTransition> BuildTransitions(
        string entityId,
        PreviewSignal previous,
        PreviewSignal next)
    {
        var transitions = new List<PreviewTransition>();
        foreach (PreviewSignal signal in EnumerateConcreteSignals())
        {
            bool wasActive = (previous & signal) != 0;
            bool isActive = (next & signal) != 0;
            if (wasActive == isActive) continue;
            transitions.Add(new PreviewTransition(entityId, signal, isActive ? PreviewTransitionType.Entered : PreviewTransitionType.Exited));
        }
        return transitions;
    }

    private static IEnumerable<PreviewSignal> EnumerateConcreteSignals()
    {
        yield return PreviewSignal.OneHitLeft;
        yield return PreviewSignal.ConnectedActive;
        yield return PreviewSignal.Pulse;
        yield return PreviewSignal.Shake;
        yield return PreviewSignal.WingFlap;
        yield return PreviewSignal.GhostTargeting;
        yield return PreviewSignal.ValidTrigger;
    }
}
