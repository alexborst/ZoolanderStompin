using System.Text.Json;
using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class GameOptionsTests
{
    [TestMethod]
    public void Defaults_match_the_mvp_unpublished_numbers()
    {
        var options = GameOptions.CreateDefault();

        Assert.AreEqual(1200, options.Easy.HitWindowMilliseconds);
        CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, options.Easy.PadsInPlay);
        Assert.AreEqual(900, options.Medium.HitWindowMilliseconds);
        CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6 }, options.Medium.PadsInPlay);
        Assert.AreEqual(650, options.Hard.HitWindowMilliseconds);
        CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6, 7 }, options.Hard.PadsInPlay);
        Assert.AreEqual(20, options.PresentationsPerRound);
        Assert.AreEqual(2, options.RoundCount);
        Assert.IsTrue(options.PreventConsecutiveRepeat);
        Assert.AreEqual(250, options.InterTargetGapMilliseconds);
        Assert.AreEqual(60, options.WinPercentThreshold);
        Assert.AreEqual(30, options.SelectTimeoutSeconds);
        Assert.AreEqual(SelectTimeoutAction.AutoStartEasy, options.SelectTimeoutAction);
        Assert.AreEqual(30, options.DebounceMilliseconds);
        Assert.AreEqual(PayoutMode.PercentageTable, options.Payout.Mode);
        Assert.AreEqual(0, options.Payout.TicketsForHitPercent(0));
        Assert.AreEqual(2, options.Payout.TicketsForHitPercent(50));
        Assert.AreEqual(8, options.Payout.TicketsForHitPercent(100));
    }

    [TestMethod]
    public void Host_appsettings_loads_without_a_recompile_and_matches_defaults()
    {
        var json = File.ReadAllText("host-appsettings.json");
        using var document = JsonDocument.Parse(json);
        var gameJson = document.RootElement.GetProperty("Game").GetRawText();
        var fromFile = GameOptions.FromJson(gameJson);
        var defaults = GameOptions.CreateDefault();

        Assert.AreEqual(defaults.Easy.HitWindowMilliseconds, fromFile.Easy.HitWindowMilliseconds);
        Assert.AreEqual(defaults.RoundCount, fromFile.RoundCount);
        Assert.AreEqual(defaults.Payout.TicketsForHitPercent(100), fromFile.Payout.TicketsForHitPercent(100));
    }

    [TestMethod]
    public void Changing_easy_hit_window_in_json_is_picked_up()
    {
        var json = """
            {
              "debounceMilliseconds": 30,
              "presentationsPerRound": 20,
              "roundCount": 2,
              "preventConsecutiveRepeat": true,
              "interTargetGapMilliseconds": 250,
              "winPercentThreshold": 60,
              "selectTimeoutSeconds": 30,
              "selectTimeoutAction": "AutoStartEasy",
              "easy": { "padsInPlay": [1, 2, 3, 4], "hitWindowMilliseconds": 1500 },
              "medium": { "padsInPlay": [1, 2, 3, 4, 5, 6], "hitWindowMilliseconds": 900 },
              "hard": { "padsInPlay": [1, 2, 3, 4, 5, 6, 7], "hitWindowMilliseconds": 650 },
              "payout": {
                "mode": "PercentageTable",
                "fixedTickets": 0,
                "table": [
                  { "minPercentInclusive": 0, "maxPercentInclusive": 100, "tickets": 1 }
                ]
              }
            }
            """;

        var options = GameOptions.FromJson(json);
        Assert.AreEqual(1500, options.Easy.HitWindowMilliseconds);
        Assert.AreEqual(4, options.Easy.Pads.Count);
    }

    [TestMethod]
    public void Rejects_a_pad_outside_one_through_seven()
    {
        var options = GameOptions.CreateDefault();
        options.Easy.PadsInPlay = [1, 2, 3, 8];

        var ex = Assert.ThrowsException<GameConfigurationException>(options.EnsureValid);
        StringAssert.Contains(ex.Message, "pad 8");
    }

    [TestMethod]
    public void Rejects_a_non_positive_hit_window()
    {
        var options = GameOptions.CreateDefault();
        options.Medium.HitWindowMilliseconds = 0;

        var ex = Assert.ThrowsException<GameConfigurationException>(options.EnsureValid);
        StringAssert.Contains(ex.Message, "HitWindowMilliseconds");
    }

    [TestMethod]
    public void Rejects_a_payout_table_with_a_gap()
    {
        var options = GameOptions.CreateDefault();
        options.Payout.Table =
        [
            new PayoutBand { MinPercentInclusive = 0, MaxPercentInclusive = 50, Tickets = 1 },
            new PayoutBand { MinPercentInclusive = 60, MaxPercentInclusive = 100, Tickets = 2 },
        ];

        var ex = Assert.ThrowsException<GameConfigurationException>(options.EnsureValid);
        StringAssert.Contains(ex.Message, "gap or overlap");
    }

    [TestMethod]
    public void Fixed_payout_ignores_the_table()
    {
        var options = GameOptions.CreateDefault();
        options.Payout.Mode = PayoutMode.Fixed;
        options.Payout.FixedTickets = 3;

        options.EnsureValid();
        Assert.AreEqual(3, options.Payout.TicketsForHitPercent(0));
        Assert.AreEqual(3, options.Payout.TicketsForHitPercent(100));
    }
}
