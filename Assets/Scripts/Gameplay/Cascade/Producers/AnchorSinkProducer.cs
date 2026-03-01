using System.Collections.Generic;

public class AnchorSinkProducer : IFillStepProducer
{
    public FillStepPhase Phase => FillStepPhase.PostFill;

    public void CollectSteps(CascadeContext context, List<FillStep> outSteps)
    {
        if (context == null || outSteps == null) return;

        var anchors = new List<string>();
        foreach (var dot in context.Board.GetDotsOnBoard())
        {
            if (dot == null) continue;
            if (dot.Dot.DotType != DotType.Anchor) continue;
            if (!context.Board.IsAtBottomOfBoard(dot.Dot.GridPosition)) continue;
            anchors.Add(dot.Dot.ID);
        }

        if (anchors.Count == 0) return;

        outSteps.Add(new FillStep(
            FillStepType.AnchorSink,
            FillStepPriority.High,
            FillStepPhase.PostFill,
            anchors,
            source: "AnchorSink"));
    }
}
