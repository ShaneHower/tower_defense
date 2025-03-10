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
        public int startGold { get; set;}
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
        public int levelHealth;
        public int currentGold;
        public Dictionary<string, List<SpawnData>> waves;
        public List<string> wavesToGo;
        public string currentWave = "1";
        public bool waveActive;
        public string enemyPrefabLoc = "res://prefabs/enemies";
        public string levelConfigLoc = "scripts/GameManager/LevelConfigs";
        private LevelData levelData;
        private Control waveHud;
        private Path2D levelPath;
        private Button waveButton;
        private UI ui;
        private Area2D endArea;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
            // Hud nodes
            ui = GetNode<UI>("HUD");
            waveHud = ui.waveHud;

            // Other Nodes
            levelPath = GetNode<Path2D>("Path2D");
            endArea = levelPath.GetNode<Area2D>("End");
            //ui = new();

            // Init work
            ParseLevelConfig();
            CreateWaveButton();
            ui.UpdateGoldValue(currentGold);
            ui.UpdateHealthValue(levelHealth);
            GameCoordinator.Instance.currentGold = currentGold;
		}

        public override void _Process(double delta)
        {
            int currentActiveEnemies = GameCoordinator.Instance.activeEnemies.Count;

            if(GameCoordinator.Instance.enemyBreach)
            {
                int breachNum = GameCoordinator.Instance.breachNum;
                if (levelHealth <= breachNum)
                {
                    GD.Print("GAME OVER");
                }
                int health = levelHealth - breachNum;
                ui.UpdateHealthValue(health);
                GameCoordinator.Instance.enemyBreach = false;
            }

            if(waveActive && currentActiveEnemies == 0)
            {
                waveActive = false;
                CreateWaveButton();
            }

            if(GameCoordinator.Instance.currentGold != currentGold)
            {
                currentGold = GameCoordinator.Instance.currentGold;
                ui.UpdateGoldValue(currentGold);
            }
        }

        private void ParseLevelConfig()
        {
            levelId = (int)GetMeta("levelId");
            string json = File.ReadAllText($"{levelConfigLoc}/level{levelId}.json");
            levelData =  JsonSerializer.Deserialize<LevelData>(json);
            levelHealth = levelData.levelHealth;
            currentGold = levelData.startGold;
            waves = levelData.waves;
            wavesToGo = waves.Keys.ToList();
        }

        public void CreateWaveButton()
        {
            string name = $"Start Wave {currentWave}";
            waveButton = ui.CreateButton(waveHud, name);
            waveButton.Pressed += OnButtonDown;
        }

        private async Task SpawnWave()
        {
            waveActive = true;

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
            waveButton.QueueFree();
            await SpawnWave();
            wavesToGo.Remove(currentWave);
            currentWave = wavesToGo.Min();
        }

	}
}
