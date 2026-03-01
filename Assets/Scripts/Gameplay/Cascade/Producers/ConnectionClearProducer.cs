using System.Collections.Generic;

public class ConnectionClearProducer : IFillStepProducer
{
    public FillStepPhase Phase => FillStepPhase.PreGravity;

    public void CollectSteps(CascadeContext context, List<FillStep> outSteps)
    {
        if (context == null || outSteps == null) return;
        if (!context.TryConsumeConnectionPayload(out var payload)) return;
        if (payload == null || payload.DotIds == null || payload.DotIds.Count < 2) return;

        outSteps.Add(new FillStep(
            FillStepType.ConnectionClear,
            FillStepPriority.VeryHigh,
            FillStepPhase.PreGravity,
            payload.DotIds,
            source: "Connection"));
    }
}
