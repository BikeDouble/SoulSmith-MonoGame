using SoulSmith.Asset;
using System.Text.Json;

namespace SoulSmith.Tests
{
    [TestClass]
    public class AssetManifestJsonSerializerTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestMethod1()
        { 
            string filepath = "../../../TestAssets/assetManifest.json";
            string jsonString = File.ReadAllText(filepath);
            AssetManifest manifest = JsonSerializer.Deserialize<AssetManifest>(jsonString);
            Assert.IsNotNull(manifest);
            TestContext.WriteLine("Test passed with dictionary: ");
            
            foreach (KeyValuePair<string, string> pair in manifest.Manifest)
            {
                TestContext.WriteLine(pair.Key + " -> " + pair.Value);
            }
        }
    }
}