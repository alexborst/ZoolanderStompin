using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class ButtonEdgesTests
{
    [TestMethod]
    public void A_held_button_is_a_rising_edge_only_on_the_first_observe()
    {
        var edges = new ButtonEdges();
        var held = new GameIoInput(
            padsHeld: [],
            easyHeld: true,
            mediumHeld: false,
            hardHeld: false,
            creditHeld: true,
            serviceCreditHeld: false,
            ticketNotchHeld: false);

        edges.Observe(held);
        Assert.IsTrue(edges.Easy);
        Assert.IsTrue(edges.Credit);

        edges.Observe(held);
        Assert.IsFalse(edges.Easy);
        Assert.IsFalse(edges.Credit);
    }

    [TestMethod]
    public void DifficultyPress_prefers_easy_when_multiple_are_pressed()
    {
        var edges = new ButtonEdges();
        edges.Observe(new GameIoInput(
            padsHeld: [],
            easyHeld: true,
            mediumHeld: true,
            hardHeld: true,
            creditHeld: false,
            serviceCreditHeld: false,
            ticketNotchHeld: false));

        Assert.AreEqual(Difficulty.Easy, edges.DifficultyPress);
    }
}
