using SoulSmith.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoulSmith.Move
{
    public class MoveLoader : IBasicAssetLoader
    {
        public IAsset Load(string path)
        {
            if (!File.Exists(path)) return null;

            string fileText = File.ReadAllText(path);

            Move move = JsonSerializer.Deserialize<Move>(fileText);

            return move;
        }
    }
}
