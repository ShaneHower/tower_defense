using System.Collections.Generic;

namespace GameNamespace.DataBase
{
    public class LevelData
    {
        public int levelId { get; set; }
        public int levelHealth { get; set; }
        public int startGold { get; set; }
        public Dictionary<string, List<SpawnData>> waves { get; set; }
    }

    public class SpawnData
    {
        public string name { get; set; }
        public string multiplier { get; set; }
    }
}
