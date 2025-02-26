namespace GameNamespace.GameManager
{
    using GameNamespace.Enemies;
    using Godot;
	using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Transactions;


    public class LevelData
    {
        public int levelId { get; set; }
        public int levelHealth { get; set; }
        public Dictionary<string, List<SpawnData>> waves { get; set; }
    }

    public class SpawnData
    {
        public string name { get; set; }
        public string multiplier { get; set;}
    }

    public partial class Level : Node
	{
        public int levelId;
        public Dictionary<string, List<SpawnData>> waves;
        public List<string> wavesToGo;
        public string currentWave = null;
        public string enemyPrefabLoc = "res://prefabs/enemies";
        public string levelConfigLoc = "scripts/GameManager/LevelConfigs";
        private LevelData levelData;
        private Control window;
        private Path2D levelPath;
        private Button waveButton;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
            window = GetNode<Control>("Control");
            levelPath = GetNode<Path2D>("Path2D");

            ParseLevelConfig();

            UI ui = new();
            waveButton = ui.CreateButton(window, $"Start!");
            waveButton.Pressed += OnButtonDown;
		}

        public override void _Process(double delta)
        {
            GD.Print(GameCoordinator.Instance.activeEnemies.Count);
        }

        private void ParseLevelConfig()
        {
            levelId = (int)GetMeta("levelId");
            string json = File.ReadAllText($"{levelConfigLoc}/level{levelId}.json");
            levelData =  JsonSerializer.Deserialize<LevelData>(json);
            waves = levelData.waves;
            wavesToGo = waves.Keys.ToList();
        }

        private async Task SpawnWave()
        {
            // Originally I used a timer to trigger the waves.  This caused enemies to spawn ontop
            // of eachother. Instead I'm staggering the spawn every second.
            List<SpawnData> waveData = waves[currentWave];
            foreach(SpawnData spawnData in waveData)
            {
                int multiplier = int.Parse(spawnData.multiplier);
                for (int i= 1; i <= multiplier; i++)
                {
                    SpawnEnemy(spawnData.name);
                    await Task.Delay(1000);
                }
            }
        }

		public void SpawnEnemy(string enemyName)
		{
			PackedScene prefab = GD.Load<PackedScene>($"{enemyPrefabLoc}/{enemyName}.tscn");
			PathFollow2D enemyPathFollow = (PathFollow2D) prefab.Instantiate();
            levelPath.AddChild(enemyPathFollow);
            Enemy enemy = enemyPathFollow.GetNode<Enemy>(enemyName);

            // Pass data to the game coordinator
            GameCoordinator.Instance.activeEnemies.Add(enemy);
		}

        public async void OnButtonDown()
        {

            if(currentWave != null)
            {
                wavesToGo.Remove(currentWave);
            }

            currentWave = wavesToGo.Min();
            waveButton.QueueFree();
            await SpawnWave();
        }

	}
}
