using System.Collections.Generic;

namespace GameNamespace.DataBase
{
    public class LevelData
    {
        public string levelId { get; set; }
        public int levelHealth { get; set; }
        public int startGold { get; set; }
        public Dictionary<string, List<SpawnData>> waves { get; set; }
    }

    public class SpawnData
    {
        public string enemyId { get; set; }
        public int count { get; set; }
        public float spawnDelay { get; set; }
    }
}
