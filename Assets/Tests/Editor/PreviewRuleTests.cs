using NUnit.Framework;
using UnityEngine;

public class PreviewRuleTests
{
    [Test]
    public void NestingPreviewRule_EmitsOneHitLeftShakeAndPulse()
    {
        var dot = new Dot(DotType.Nesting, Vector2Int.zero);
        dot.AddModel(new Hittable(dot, hitRule: null, hitMax: 3, hitCount: 2));
        var presenter = new TestEntityPresenter(dot);
        var context = new PreviewContext(null, null, new System.Collections.Generic.HashSet<string>(), new System.Collections.Generic.List<string>());

        var rule = new NestingPreviewRule();
        var signals = rule.Evaluate(presenter, context);

        Assert.IsTrue((signals & PreviewSignal.OneHitLeft) != 0);
        Assert.IsTrue((signals & PreviewSignal.Shake) != 0);
        Assert.IsTrue((signals & PreviewSignal.Pulse) != 0);
    }

    [Test]
    public void BeetlePreviewRule_EmitsConnectedAndWingFlapWhenInPath()
    {
        var dot = new Dot(DotType.Beetle, Vector2Int.zero);
        var presenter = new TestEntityPresenter(dot);
        var path = new System.Collections.Generic.List<string> { dot.ID };
        var context = new PreviewContext(null, null, new System.Collections.Generic.HashSet<string>(path), path);

        var rule = new BeetlePreviewRule();
        var signals = rule.Evaluate(presenter, context);

        Assert.IsTrue((signals & PreviewSignal.ConnectedActive) != 0);
        Assert.IsTrue((signals & PreviewSignal.WingFlap) != 0);
    }

    private sealed class TestEntityPresenter : EntityPresenter
    {
        public TestEntityPresenter(BoardEntity entity) : base(entity, null)
        {
        }
    }
}
