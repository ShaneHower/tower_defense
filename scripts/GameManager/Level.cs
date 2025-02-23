namespace GameNamespace.GameManager
{
    using GameNamespace.Enemies;
    using Godot;
	using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;

    public class LevelData
    {
        public int levelId { get; set; }
        public int levelTime { get; set; }
        public Dictionary<string, WaveSpawnData> waves { get; set; }
    }

    public class WaveSpawnData
    {
        public int executeTimer { get; set; }
        public List<Dictionary<string, string>> enemies { get; set;}
    }


    public partial class Level : Node
	{
        public int levelId;
        public int levelTime;
        public int elapsedTime;
        public Dictionary<string, WaveSpawnData> waves;
        public string enemyPrefabLoc = "res://prefabs/enemies";
        public string levelConfigLoc = "scripts/GameManager/LevelConfigs";
        private Timer levelTimer;
        private LevelData level;
        private Path2D enemyPath;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
            levelId = (int)GetMeta("levelId");
            string json = File.ReadAllText($"{levelConfigLoc}/level{levelId}.json");
            level = JsonSerializer.Deserialize<LevelData>(json);
            levelTime = level.levelTime;
            waves = level.waves;

            enemyPath = GetNode<Path2D>("Path2D");
            levelTimer = GetNode<Timer>("levelTimer");
            levelTimer.Timeout += OnTimerTick;
            levelTimer.Start(1.0f);
		}

        private void OnTimerTick()
        {
            GD.Print("Timing it!");
            elapsedTime++;
            foreach(var wave in waves)
            {
                if(elapsedTime == wave.Value.executeTimer)
                {
                    foreach(Dictionary<string, string> enemyData in wave.Value.enemies)
                    {
                        string name = enemyData["name"];
                        int multiplier = int.Parse(enemyData["multiplier"]);
                        // GD.Print($"Spawn {multiplier} {name}");
                        for (int i= 1; i <= multiplier; i++)
                        {
                            // GD.Print($"Spawn {i} {name}");
                            SpawnEnemy(name);
                        }

                    }
                }
            }

            if(elapsedTime >= levelTime)
            {
                GD.Print("Spawner ended");
                levelTimer.Stop();
            }
        }

		public void SpawnEnemy(string enemyName)
		{
			PackedScene prefab = GD.Load<PackedScene>($"{enemyPrefabLoc}/{enemyName}.tscn");
			PathFollow2D enemy = (PathFollow2D) prefab.Instantiate();
            enemyPath.AddChild(enemy);

            GD.Print($"Enemy Name: {enemy.Name}");
		}

	}
}
