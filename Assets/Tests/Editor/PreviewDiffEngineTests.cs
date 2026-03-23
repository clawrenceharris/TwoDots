using System.Linq;
using NUnit.Framework;

public class PreviewDiffEngineTests
{
    [Test]
    public void BuildTransitions_EmitsEnteredSignals()
    {
        var transitions = PreviewDiffEngine.BuildTransitions(
            "dot-1",
            PreviewSignal.None,
            PreviewSignal.OneHitLeft | PreviewSignal.Shake);

        Assert.AreEqual(2, transitions.Count);
        Assert.IsTrue(transitions.Any(t => t.Signal == PreviewSignal.OneHitLeft && t.Type == PreviewTransitionType.Entered));
        Assert.IsTrue(transitions.Any(t => t.Signal == PreviewSignal.Shake && t.Type == PreviewTransitionType.Entered));
    }

    [Test]
    public void BuildTransitions_EmitsExitedSignals()
    {
        var transitions = PreviewDiffEngine.BuildTransitions(
            "dot-1",
            PreviewSignal.ConnectedActive | PreviewSignal.WingFlap,
            PreviewSignal.None);

        Assert.AreEqual(2, transitions.Count);
        Assert.IsTrue(transitions.Any(t => t.Signal == PreviewSignal.ConnectedActive && t.Type == PreviewTransitionType.Exited));
        Assert.IsTrue(transitions.Any(t => t.Signal == PreviewSignal.WingFlap && t.Type == PreviewTransitionType.Exited));
    }
}
