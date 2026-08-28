using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class TimedDeadlineTests
{
    [TestMethod]
    public void Jumping_the_easy_hit_window_expires_the_deadline()
    {
        var clock = new FakeGameClock();
        var hitWindow = TimeSpan.FromMilliseconds(GameOptions.CreateDefault().Easy.HitWindowMilliseconds);
        var deadline = new TimedDeadline(clock, hitWindow);

        Assert.IsFalse(deadline.IsExpired);

        clock.Advance(hitWindow - TimeSpan.FromMilliseconds(1));
        Assert.IsFalse(deadline.IsExpired);

        clock.Advance(TimeSpan.FromMilliseconds(1));
        Assert.IsTrue(deadline.IsExpired);
    }
}
