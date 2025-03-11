namespace GameNamespace.GameManager
{
    using GameNamespace.Enemies;
    using Godot;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using System.Text.Json;
    using System.Threading.Tasks;


    public class LevelData
    {
        // JSON config for level data
        public int levelId { get; set; }
        public int levelHealth { get; set; }
        public int startGold { get; set;}
        public Dictionary<string, List<SpawnData>> waves { get; set; }
    }

    public class SpawnData
    {
        // JSON config for spawn data
        public string name { get; set; }
        public string multiplier { get; set;}
    }

    /// <summary>
    /// This object is responsible for the level behavior.  Namely it handles
    ///     1. Spawning enemies
    ///     2. Tracking level (or player) health and triggering level end game behavior
    ///     3. Tracking level (or player) gold
    /// The level class tracks (2) and (3) by reading changes from the GameCoordinator object.  It may seem like we
    /// should track player health and gold in a player object, but these values are based on the level itself.
    /// Therefore, I think it makes sense to make this object responsible.
    /// </summary>
    public partial class Level : Node
	{
        // Level meta data
        public int levelId;
        public int levelHealth;
        public int currentGold;

        // Wave data
        public Dictionary<string, List<SpawnData>> waves;
        public List<string> wavesToGo;
        public string currentWave = "1";
        public bool waveActive;

        // File and prefab locations
        public string enemyPrefabLoc = "res://prefabs/enemies";
        public string levelConfigLoc = "scripts/GameManager/LevelConfigs";

        // Game objects
        public Path2D levelPath;
        public UIControl uiControl;
        private Control waveHud;
        private Button waveButton;

		/// <summary>
        /// This is the first bit of code to run in the scene.  Establishes the base-line health, economy, and wave
        /// information.  Creates the initial wave button as well.
        /// </summary>
		public override void _Ready()
		{
            // Nodes
            CanvasLayer uiCanvas = GetNode<CanvasLayer>("UICanvas");
            uiControl = uiCanvas.GetNode<UIControl>("UI");
            levelPath = GetNode<Path2D>("LevelPath");

            // Init work
            ParseLevelConfig();
            CreateWaveButton();
            uiControl.UpdateGoldValue(currentGold);
            uiControl.UpdateHealthValue(levelHealth);

            GameCoordinator.Instance.currentGold = currentGold;
            GameCoordinator.Instance.level = this;
		}

        /// <summary>
        /// The level object does three things
        ///     1. Track the level health. If enemies hit the end node we need to subtract from health.  This is where
        ///        we will trigger an end game state.
        ///     2. Handle wave state.  When there are no more enemies on the board, it generates a wave button that
        ///        will spawn next wave.
        ///     3. Track the level Gold. Enemy deaths will grant gold, placing towers will remove gold. These objects are
        ///        updating the Game coordinator when these events trigger. It's this objects job to update that value on
        ///        the UI.
        /// </summary>
        /// <param name="delta"></param>
        public override void _Process(double delta)
        {
            // Track health.
            if(GameCoordinator.Instance.enemyBreach)
            {
                int breachNum = GameCoordinator.Instance.breachNum;
                if (levelHealth <= breachNum)
                {
                    GD.Print("GAME OVER");
                }
                int health = levelHealth - breachNum;
                uiControl.UpdateHealthValue(health);
                GameCoordinator.Instance.enemyBreach = false;
            }

            // Track wave state.
            int currentActiveEnemies = GameCoordinator.Instance.activeEnemies.Count;
            if(waveActive && currentActiveEnemies == 0)
            {
                waveActive = false;
                CreateWaveButton();
            }

            // Track gold.
            if(GameCoordinator.Instance.currentGold != currentGold)
            {
                currentGold = GameCoordinator.Instance.currentGold;
                uiControl.UpdateGoldValue(currentGold);
            }
        }

        /// <summary>
        /// We store the level info in a JSON config.  This allows us to easily spin up and fine tune level specifics
        /// without touching code.
        /// </summary>
        private void ParseLevelConfig()
        {
            levelId = (int)GetMeta("levelId");
            string json = File.ReadAllText($"{levelConfigLoc}/level{levelId}.json");
            LevelData levelData =  JsonSerializer.Deserialize<LevelData>(json);
            levelHealth = levelData.levelHealth;
            currentGold = levelData.startGold;
            waves = levelData.waves;
            wavesToGo = waves.Keys.ToList();
        }

        /// <summary>
        /// This method pings the UI object to create our Wave button.  It then maps it to some custom behavior.
        /// </summary>
        public void CreateWaveButton()
        {
            string name = $"Start Wave {currentWave}";
            waveButton = uiControl.CreateWaveButton(name);
            waveButton.Pressed += OnWaveButton;
        }

        /// <summary>
        /// This method spawns an enemy every second. There can be multiple instances of an enemy in spawnData.
        /// For example, we may spawn 3 ghosts one after another.  This is defined as {"name": "ghost", "multiplier": "3"}
        /// in the JSON config.
        /// </summary>
        /// <returns></returns>
        private async Task SpawnWave()
        {
            waveActive = true;
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
            // Spawn a single enemy to the level's path.
			PackedScene prefab = GD.Load<PackedScene>($"{enemyPrefabLoc}/{enemyName}.tscn");
			PathFollow2D enemyPathFollow = (PathFollow2D) prefab.Instantiate();
            levelPath.AddChild(enemyPathFollow);
            Enemy enemy = enemyPathFollow.GetNode<Enemy>(enemyName);

            // Pass data to the game coordinator
            GameCoordinator.Instance.activeEnemies.Add(enemy);
		}

        public async void OnWaveButton()
        {
            waveButton.QueueFree();
            await SpawnWave();
            wavesToGo.Remove(currentWave);
            currentWave = wavesToGo.Min();
        }

	}
}
