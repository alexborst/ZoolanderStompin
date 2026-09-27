using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class RoundRatingTests
{
    [TestMethod]
    public void Frozen_thresholds_split_twenty_presentations_into_four_tiers()
    {
        var options = GameOptions.CreateDefault();

        Assert.AreEqual(RoundPerformance.Bad, RoundRating.Rate(0, options));
        Assert.AreEqual(RoundPerformance.Bad, RoundRating.Rate(4, options));
        Assert.AreEqual(RoundPerformance.Medium, RoundRating.Rate(5, options));
        Assert.AreEqual(RoundPerformance.Medium, RoundRating.Rate(15, options));
        Assert.AreEqual(RoundPerformance.Good, RoundRating.Rate(16, options));
        Assert.AreEqual(RoundPerformance.Good, RoundRating.Rate(19, options));
        Assert.AreEqual(RoundPerformance.Perfect, RoundRating.Rate(20, options));
    }

    [TestMethod]
    public void Each_tier_maps_to_its_own_round_end_cue_and_folder()
    {
        Assert.AreEqual(GameSound.RoundEndBad, RoundRating.Sound(RoundPerformance.Bad));
        Assert.AreEqual(GameSound.RoundEndMedium, RoundRating.Sound(RoundPerformance.Medium));
        Assert.AreEqual(GameSound.RoundEndGood, RoundRating.Sound(RoundPerformance.Good));
        Assert.AreEqual(GameSound.RoundEndPerfect, RoundRating.Sound(RoundPerformance.Perfect));

        Assert.AreEqual("round-end/bad", SoundFolders.For(GameSound.RoundEndBad));
        Assert.AreEqual("round-end/medium", SoundFolders.For(GameSound.RoundEndMedium));
        Assert.AreEqual("round-end/good", SoundFolders.For(GameSound.RoundEndGood));
        Assert.AreEqual("round-end/perfect", SoundFolders.For(GameSound.RoundEndPerfect));
    }

    [TestMethod]
    public void Rejects_good_threshold_below_medium()
    {
        var options = GameOptions.CreateDefault();
        options.RoundEndGoodMinimumHits = 4;

        var ex = Assert.ThrowsException<GameConfigurationException>(options.EnsureValid);
        StringAssert.Contains(ex.Message, "RoundEndGoodMinimumHits");
    }
}
