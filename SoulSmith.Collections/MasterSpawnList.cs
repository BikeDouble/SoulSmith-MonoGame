using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Collections
{
    /// <summary>
    /// Contains all the 
    /// </summary>
    public class MasterSpawnList
    {
        private Dictionary<(int Difficulty, int Area), SoulSmithWeightedList<string>> _passiveSpawns;
        private Dictionary<int, SoulSmithWeightedList<string>> _eliteSpawns;
        private Dictionary<int, SoulSmithWeightedList<string>> _bossSpawns;

        public MasterSpawnList(Dictionary<(int Difficulty, int Area), SoulSmithWeightedList<string>> passiveSpawnLists,
            Dictionary<int, SoulSmithWeightedList<string>> eliteSpawnLists,
            Dictionary<int, SoulSmithWeightedList<string>> bossSpawnLists)
        {
            _passiveSpawns = passiveSpawnLists;
            _eliteSpawns = eliteSpawnLists;
            _bossSpawns = bossSpawnLists;
        }

        public string SelectPassiveSpawn(int difficulty, int area)
        {
            (int Difficulty, int Area) key = (difficulty, area);

            SoulSmithWeightedList<string> list = _passiveSpawns.GetValueOrDefault(key);

            if (list == null) throw new Exception("No spawn list found.");

            if (list.Count < 1) throw new Exception("Spawn list is empty.");

            return list.Next();
        }

        public string SelectEliteSpawn(int area)
        {
            SoulSmithWeightedList<string> list = _eliteSpawns.GetValueOrDefault(area);

            if (list == null) throw new Exception("No spawn list found.");

            if (list.Count < 1) throw new Exception("Spawn list is empty.");

            return list.Next();
        }

        public string SelectBossSpawn(int area)
        {
            SoulSmithWeightedList<string> list = _bossSpawns.GetValueOrDefault(area);

            if (list == null) throw new Exception("No spawn list found.");

            if (list.Count < 1) throw new Exception("Spawn list is empty.");

            return list.Next();
        }
    }
}
