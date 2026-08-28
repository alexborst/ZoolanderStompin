using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class ScoreTests
{
    [TestMethod]
    public void Hit_percent_is_null_when_nothing_is_resolved()
    {
        Assert.IsNull(new Score(0, 0).HitPercent);
    }

    [TestMethod]
    public void Hit_percent_uses_only_resolved_presentations()
    {
        var percent = new Score(hits: 3, misses: 1).HitPercent;
        Assert.AreEqual(75.0, percent);
    }
}
