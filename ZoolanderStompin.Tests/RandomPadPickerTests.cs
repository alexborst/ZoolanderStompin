using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class RandomPadPickerTests
{
    [TestMethod]
    public void Picks_only_from_the_candidate_set()
    {
        var picker = new RandomPadPicker(new Random(7));
        var candidates = new FloorPad[] { new(2), new(4), new(6) };

        for (var i = 0; i < 40; i++)
        {
            var pick = picker.Next(candidates);
            Assert.IsTrue(candidates.Contains(pick));
        }
    }

    [TestMethod]
    public void Returns_the_only_candidate()
    {
        var picker = new RandomPadPicker(new Random(1));
        var only = new FloorPad(5);

        Assert.AreEqual(only, picker.Next([only]));
        Assert.AreEqual(only, picker.Next([only]));
    }

    [TestMethod]
    public void Rejects_an_empty_candidate_set()
    {
        var picker = new RandomPadPicker();
        Assert.ThrowsException<ArgumentException>(() => picker.Next([]));
    }
}
