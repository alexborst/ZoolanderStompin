namespace ZoolanderStompin.Tests;

[TestClass]
public class FakeGameClockTests
{
    [TestMethod]
    public void Advance_jumps_now_without_waiting()
    {
        var clock = new FakeGameClock();

        clock.Advance(TimeSpan.FromMilliseconds(1200));

        Assert.AreEqual(TimeSpan.FromMilliseconds(1200), clock.Now);
    }

    [TestMethod]
    public async Task Delay_advances_now_immediately()
    {
        var clock = new FakeGameClock();

        await clock.Delay(TimeSpan.FromMilliseconds(50));

        Assert.AreEqual(TimeSpan.FromMilliseconds(50), clock.Now);
    }
}
