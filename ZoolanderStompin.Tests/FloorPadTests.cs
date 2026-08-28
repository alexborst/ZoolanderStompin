using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class FloorPadTests
{
    [TestMethod]
    public void Accepts_pads_one_through_seven()
    {
        for (var i = 1; i <= FloorPad.Count; i++)
        {
            Assert.AreEqual(i, new FloorPad(i).Number);
        }
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(8)]
    public void Rejects_pads_outside_one_through_seven(int number)
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new FloorPad(number));
    }
}
