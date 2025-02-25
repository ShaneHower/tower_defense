namespace GameNamespace.GameManager
{
    using GameNamespace.Enemies;
    using Godot;
	using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class LevelData
    {
        public int levelId { get; set; }
        public Dictionary<string, WaveSpawnData> waves { get; set; }
    }

    public class WaveSpawnData
    {
        public List<Dictionary<string, string>> enemies { get; set;}
    }


    public partial class Level : Node
	{
        public int levelId;
        public Dictionary<string, WaveSpawnData> waves;
        public string enemyPrefabLoc = "res://prefabs/enemies";
        public string levelConfigLoc = "scripts/GameManager/LevelConfigs";
        private LevelData level;
        private Path2D enemyPath;

		// Called when the node enters the scene tree for the first time.
		public override async void _Ready()
		{
            enemyPath = GetNode<Path2D>("Path2D");
            ParseLevelConfig();
            await SpawnWaves();
		}

        private void ParseLevelConfig()
        {
            levelId = (int)GetMeta("levelId");
            string json = File.ReadAllText($"{levelConfigLoc}/level{levelId}.json");
            level =  JsonSerializer.Deserialize<LevelData>(json);
            waves = level.waves;
        }

        private async Task SpawnWaves()
        {
            // Originally I used a timer to trigger the waves.  This caused enemies to spawn ontop
            // of eachother. Instead I'm staggering the spawn every second.
            foreach(var wave in waves)
            {
                foreach(Dictionary<string, string> enemyData in wave.Value.enemies)
                {
                    string name = enemyData["name"];
                    int multiplier = int.Parse(enemyData["multiplier"]);

                    for (int i= 1; i <= multiplier; i++)
                    {
                        SpawnEnemy(name);
                        await Task.Delay(1000);
                    }
                }
            }
        }

		public void SpawnEnemy(string enemyName)
		{
			PackedScene prefab = GD.Load<PackedScene>($"{enemyPrefabLoc}/{enemyName}.tscn");
			PathFollow2D enemy = (PathFollow2D) prefab.Instantiate();
            enemyPath.AddChild(enemy);
		}

	}
}
