using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class FakeGameIoTests
{
    [TestMethod]
    public void Read_returns_the_queued_input()
    {
        var io = new FakeGameIo
        {
            NextInput = new GameIoInput(
                padsHeld: [new FloorPad(3)],
                easyHeld: false,
                mediumHeld: true,
                hardHeld: false,
                creditHeld: true,
                serviceCreditHeld: false,
                ticketNotchHeld: false),
        };

        var input = io.Read();

        Assert.AreEqual(1, io.ReadCount);
        Assert.IsTrue(input.IsPadHeld(new FloorPad(3)));
        Assert.IsTrue(input.MediumHeld);
        Assert.IsTrue(input.CreditHeld);
    }

    [TestMethod]
    public void Apply_records_lamp_output()
    {
        var io = new FakeGameIo();
        var output = new GameIoOutput(
            padLampsOn: [new FloorPad(1), new FloorPad(4)],
            easyLampOn: true,
            mediumLampOn: false,
            hardLampOn: false,
            pictorialLampsOn: [true, false, false, false],
            scoreDigits: 7,
            ticketDigits: null,
            sound: GameSound.Coin,
            ticketEnable: false);

        io.Apply(output);

        Assert.IsNotNull(io.LastOutput);
        Assert.IsTrue(io.LastOutput.IsPadLampOn(new FloorPad(1)));
        Assert.IsTrue(io.LastOutput.EasyLampOn);
        Assert.AreEqual(7, io.LastOutput.ScoreDigits);
        Assert.AreEqual(GameSound.Coin, io.LastOutput.Sound);
    }
}
