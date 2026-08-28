using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class PictorialMeterTests
{
    [TestMethod]
    public void Zero_hits_leaves_all_lamps_off()
    {
        CollectionAssert.AreEqual(
            new[] { false, false, false, false },
            PictorialMeter.Lamps(0, 40).ToArray());
    }

    [TestMethod]
    public void The_first_hit_lights_the_first_lamp()
    {
        CollectionAssert.AreEqual(
            new[] { true, false, false, false },
            PictorialMeter.Lamps(1, 40).ToArray());
    }

    [TestMethod]
    public void Lamps_fill_across_the_session_and_saturate_at_four()
    {
        CollectionAssert.AreEqual(
            new[] { true, false, false, false },
            PictorialMeter.Lamps(10, 40).ToArray());
        CollectionAssert.AreEqual(
            new[] { true, true, false, false },
            PictorialMeter.Lamps(11, 40).ToArray());
        CollectionAssert.AreEqual(
            new[] { true, true, true, true },
            PictorialMeter.Lamps(40, 40).ToArray());
        CollectionAssert.AreEqual(
            new[] { true, true, true, true },
            PictorialMeter.Lamps(99, 40).ToArray());
    }
}
