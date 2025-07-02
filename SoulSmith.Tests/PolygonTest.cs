using Microsoft.Xna.Framework;
using System.Text.Json;
using SoulSmith.Shapes;
using SoulSmith.Core;

namespace SoulSmith.Tests
{
    [TestClass]
    public class PolygonTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void ContainsTest()
        {
            Vector2[] vertices = {new Vector2(-50, -50), new Vector2(50, -50), new Vector2(50, 50), new Vector2(-50, 50)};
            Polygon square = new Polygon(vertices);
            bool contains = square.ContainsLocal(new Vector2(0, 0));
            Assert.IsTrue(contains);

            contains = square.ContainsLocal(new Vector2(51, 25));
            Assert.IsFalse(contains);

            contains = square.ContainsLocal(new Vector2(-40, 0));
            Assert.IsTrue(contains);

            Position position = new Position(20, 20, 1, 1, 0);

            contains = square.ContainsGlobal(new Vector2(-45, -45), position);
            Assert.IsFalse(contains);

            contains = square.ContainsGlobal(new Vector2(139, -87), position);
            Assert.IsFalse(contains);

            position = new Position(0, 0, 4, 4, 0);

            contains = square.ContainsGlobal(new Vector2(139, -87), position);
            Assert.IsTrue(contains);

            TestContext.WriteLine("Test passed");
        }
    }
}