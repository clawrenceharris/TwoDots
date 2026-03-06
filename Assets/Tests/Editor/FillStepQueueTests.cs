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

    [Test]
    public void FillStepQueue_OrdersVeryHighBeforeHighBeforeNormalBeforeLow()
    {
        var queue = new FillStepQueue();
        int sequence = 0;

        queue.Enqueue(new FillStep(FillStepType.GemExplode, FillStepPriority.Normal, FillStepPhase.PostFill), ref sequence);
        queue.Enqueue(new FillStep(FillStepType.BombExplode, FillStepPriority.VeryHigh, FillStepPhase.PostFill), ref sequence);
        queue.Enqueue(new FillStep(FillStepType.AnchorSink, FillStepPriority.High, FillStepPhase.PostFill), ref sequence);
        queue.Enqueue(new FillStep(FillStepType.ConnectionClear, FillStepPriority.Low, FillStepPhase.PreGravity), ref sequence);

        Assert.IsTrue(queue.TryDequeue(out var first));
        Assert.IsTrue(queue.TryDequeue(out var second));
        Assert.IsTrue(queue.TryDequeue(out var third));
        Assert.IsTrue(queue.TryDequeue(out var fourth));

        Assert.AreEqual(FillStepPriority.VeryHigh, first.Priority);
        Assert.AreEqual(FillStepPriority.High, second.Priority);
        Assert.AreEqual(FillStepPriority.Normal, third.Priority);
        Assert.AreEqual(FillStepPriority.Low, fourth.Priority);
    }
}
