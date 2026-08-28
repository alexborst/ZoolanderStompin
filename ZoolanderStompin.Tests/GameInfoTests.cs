using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class GameInfoTests
{
    [TestMethod]
    public void Name_is_the_product_title()
    {
        Assert.AreEqual("Zoolander Stompin'", GameInfo.Name);
    }
}
