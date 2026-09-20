using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class GameIoInputTests
{
    [TestMethod]
    public void Combine_unions_pads_and_ors_buttons()
    {
        var left = new GameIoInput(
            padsHeld: [new FloorPad(1)],
            easyHeld: true,
            mediumHeld: false,
            hardHeld: false,
            creditHeld: false,
            serviceCreditHeld: true,
            ticketNotchHeld: false);
        var right = new GameIoInput(
            padsHeld: [new FloorPad(1), new FloorPad(7)],
            easyHeld: false,
            mediumHeld: true,
            hardHeld: false,
            creditHeld: true,
            serviceCreditHeld: false,
            ticketNotchHeld: false);

        var combined = left.Combine(right);

        Assert.IsTrue(combined.IsPadHeld(new FloorPad(1)));
        Assert.IsTrue(combined.IsPadHeld(new FloorPad(7)));
        Assert.IsTrue(combined.EasyHeld);
        Assert.IsTrue(combined.MediumHeld);
        Assert.IsTrue(combined.CreditHeld);
        Assert.IsTrue(combined.ServiceCreditHeld);
        Assert.IsFalse(combined.HardHeld);
        Assert.IsFalse(combined.TicketNotchHeld);
    }
}
