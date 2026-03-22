using System.Collections.Generic;
using System.Linq;
/// <summary>
/// Pre-gravity producer that consumes the connection payload once per cascade and enqueues
/// a single step to clear all dots in the completed path. Runs at VeryHigh priority so
/// connection clears happen first; other producers (e.g. seed, hedgehog) then react to recent clears.
/// </summary>
public class ConnectionClearProducer : IFillStepProducer
{
        public FillStepPhase Phase => FillStepPhase.PreGravity;

        public void CollectSteps(CascadeContext context, List<FillStep> outSteps)
        {
                if (context == null || outSteps == null) return;
                if (!context.TryConsumeConnectionPayload(out var payload)) return;
                if (payload.HasValue && (payload.Value.DotIdsInPath == null || payload.Value.DotIdsInPath.Count < 2)) return;
                
                outSteps.Add(new FillStep(
                        FillStepType.ConnectionClear,
                        FillStepPriority.VeryHigh,
                        FillStepPhase.PreGravity,
                        toHit:payload.Value.DotIdsInPath.Concat(payload.Value.DotsToHitFromSquare).Distinct().ToList(),
                        source: "Connection"));
        }

}
