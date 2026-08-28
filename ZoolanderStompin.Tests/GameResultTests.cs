using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class GameResultTests
{
    [TestMethod]
    public void Zero_percent_is_a_loss_with_table_tickets()
    {
        var result = GameResult.Evaluate(new Score(0, 10), GameOptions.CreateDefault());

        Assert.AreEqual(0, result.HitPercent);
        Assert.IsFalse(result.Won);
        Assert.AreEqual(0, result.Tickets);
    }

    [TestMethod]
    public void One_hundred_percent_is_a_win_with_max_table_tickets()
    {
        var result = GameResult.Evaluate(new Score(10, 0), GameOptions.CreateDefault());

        Assert.AreEqual(100, result.HitPercent);
        Assert.IsTrue(result.Won);
        Assert.AreEqual(8, result.Tickets);
    }

    [TestMethod]
    public void Win_line_is_at_the_configured_threshold()
    {
        var options = GameOptions.CreateDefault();
        var lose = GameResult.Evaluate(new Score(59, 41), options);
        var win = GameResult.Evaluate(new Score(60, 40), options);

        Assert.IsFalse(lose.Won);
        Assert.AreEqual(2, lose.Tickets);
        Assert.IsTrue(win.Won);
        Assert.AreEqual(4, win.Tickets);
    }

    [TestMethod]
    public void Fixed_payout_ignores_hit_percentage_for_tickets()
    {
        var options = GameOptions.CreateDefault();
        options.Payout.Mode = PayoutMode.Fixed;
        options.Payout.FixedTickets = 3;

        var missAll = GameResult.Evaluate(new Score(0, 8), options);
        var hitAll = GameResult.Evaluate(new Score(8, 0), options);

        Assert.AreEqual(3, missAll.Tickets);
        Assert.IsFalse(missAll.Won);
        Assert.AreEqual(3, hitAll.Tickets);
        Assert.IsTrue(hitAll.Won);
    }
}
