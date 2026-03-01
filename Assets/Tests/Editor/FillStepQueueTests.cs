using NUnit.Framework;

public class FillStepQueueTests
{
    [Test]
    public void FillStepQueue_OrdersByPriorityThenSequence()
    {
        var queue = new FillStepQueue();
        int sequence = 0;

        queue.Enqueue(new FillStep(FillStepType.ConnectionClear, FillStepPriority.Low, FillStepPhase.PreGravity), ref sequence);
        queue.Enqueue(new FillStep(FillStepType.ConnectionClear, FillStepPriority.High, FillStepPhase.PreGravity), ref sequence);
        queue.Enqueue(new FillStep(FillStepType.ConnectionClear, FillStepPriority.High, FillStepPhase.PreGravity), ref sequence);

        Assert.IsTrue(queue.TryDequeue(out var first));
        Assert.IsTrue(queue.TryDequeue(out var second));
        Assert.IsTrue(queue.TryDequeue(out var third));

        Assert.AreEqual(FillStepPriority.High, first.Priority);
        Assert.AreEqual(FillStepPriority.High, second.Priority);
        Assert.AreEqual(FillStepPriority.Low, third.Priority);
        Assert.Less(first.Sequence, second.Sequence);
    }
}
