using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class GameIoOutputTests
{
    [TestMethod]
    public void Rejects_the_wrong_pictorial_lamp_count()
    {
        Assert.ThrowsException<ArgumentException>(() => new GameIoOutput(
            padLampsOn: [],
            easyLampOn: false,
            mediumLampOn: false,
            hardLampOn: false,
            pictorialLampsOn: [true],
            scoreDigits: null,
            ticketDigits: null,
            sound: null,
            ticketEnable: false));
    }

    [TestMethod]
    public void Rejects_score_digits_above_99()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new GameIoOutput(
            padLampsOn: [],
            easyLampOn: false,
            mediumLampOn: false,
            hardLampOn: false,
            pictorialLampsOn: GameIoOutput.Off.PictorialLampsOn,
            scoreDigits: 100,
            ticketDigits: null,
            sound: null,
            ticketEnable: false));
    }
}
