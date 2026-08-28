using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class GameSessionPayoutTests
{
    [TestMethod]
    public void Missing_every_presentation_pays_zero_tickets_and_loses()
    {
        var driver = GameSessionDriver.Scripted(pads: [1, 2, 3, 4]);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Easy);
        driver.PlayUntilResults(hitEveryPresentation: false);

        Assert.AreEqual(SessionPhase.Results, driver.Session.Phase);
        Assert.AreEqual(0, driver.Session.Score.Hits);
        Assert.AreEqual(4, driver.Session.Score.Misses);
        Assert.IsFalse(driver.Session.Result?.Won);
        Assert.AreEqual(0, driver.Session.Result?.Tickets);
        Assert.AreEqual(0, driver.Output.TicketDigits);
        Assert.IsFalse(driver.Output.TicketEnable);
        Assert.AreEqual(GameSound.GameEnd, driver.Output.Sound);
    }

    [TestMethod]
    public void Hitting_every_presentation_pays_max_tickets_and_wins()
    {
        var driver = GameSessionDriver.Scripted(pads: [1, 2, 3, 4]);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Easy);
        driver.PlayUntilResults(hitEveryPresentation: true);

        Assert.AreEqual(SessionPhase.Results, driver.Session.Phase);
        Assert.AreEqual(4, driver.Session.Score.Hits);
        Assert.AreEqual(0, driver.Session.Score.Misses);
        Assert.IsTrue(driver.Session.Result?.Won);
        Assert.AreEqual(8, driver.Session.Result?.Tickets);
        Assert.AreEqual(8, driver.Output.TicketDigits);
        Assert.IsTrue(driver.Output.TicketEnable);
        Assert.AreEqual(GameSound.Ticket, driver.Output.Sound);
    }

    [TestMethod]
    public void Fixed_payout_pays_the_same_ticket_count_win_or_lose()
    {
        var options = GameSessionDriver.CreateShortSession();
        options.Payout.Mode = PayoutMode.Fixed;
        options.Payout.FixedTickets = 3;
        var driver = GameSessionDriver.Scripted(options, 1, 2, 3, 4);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Easy);
        driver.PlayUntilResults(hitEveryPresentation: false);

        Assert.IsFalse(driver.Session.Result?.Won);
        Assert.AreEqual(3, driver.Session.Result?.Tickets);
        Assert.AreEqual(3, driver.Output.TicketDigits);
        Assert.IsTrue(driver.Output.TicketEnable);
    }
}
