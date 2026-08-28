using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class KeyboardBindingsTests
{
    [TestMethod]
    [DataRow(ConsoleKey.D1, 1)]
    [DataRow(ConsoleKey.NumPad7, 7)]
    public void Maps_digit_keys_to_floor_pads(ConsoleKey key, int number)
    {
        var pad = KeyboardBindings.TryGetFloorPad(key);
        Assert.IsNotNull(pad);
        Assert.AreEqual(number, pad.Value.Number);
    }

    [TestMethod]
    public void Ignores_unmapped_keys_for_pads()
    {
        Assert.IsNull(KeyboardBindings.TryGetFloorPad(ConsoleKey.A));
    }

    [TestMethod]
    public void Maps_difficulty_and_credit_keys()
    {
        Assert.AreEqual(Difficulty.Easy, KeyboardBindings.TryGetDifficulty(ConsoleKey.E));
        Assert.AreEqual(Difficulty.Medium, KeyboardBindings.TryGetDifficulty(ConsoleKey.M));
        Assert.AreEqual(Difficulty.Hard, KeyboardBindings.TryGetDifficulty(ConsoleKey.H));
        Assert.IsTrue(KeyboardBindings.IsCredit(ConsoleKey.C));
        Assert.IsTrue(KeyboardBindings.IsCredit(ConsoleKey.Enter));
        Assert.IsTrue(KeyboardBindings.IsServiceCredit(ConsoleKey.F));
        Assert.IsTrue(KeyboardBindings.IsTicketNotch(ConsoleKey.N));
        Assert.IsTrue(KeyboardBindings.IsQuit(ConsoleKey.Escape));
    }
}
