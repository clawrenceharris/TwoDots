using System.Collections.Generic;

public interface IFillStepProducer
{
    FillStepPhase Phase { get; }
    void CollectSteps(CascadeContext context, List<FillStep> outSteps);
}
