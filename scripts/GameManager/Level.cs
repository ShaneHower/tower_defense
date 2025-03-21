namespace GameNamespace.GameManager
{
    using GameNamespace.DataBase;
    using GameNamespace.GameAssets;
    using GameNamespace.UI;
    using Godot;
    using Serilog;

    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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
        public string levelId;
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
        private static readonly ILogger log = Log.ForContext<Level>();

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
            // Set class Vars.
            levelId = (string)GetMeta("levelId");
            SetVars();

            // Set game objects.
            CanvasLayer uiCanvas = GetNode<CanvasLayer>("UICanvas");
            uiControl = uiCanvas.GetNode<UIControl>("UI");
            levelPath = GetNode<Path2D>("LevelPath");

            // Init work.
            CreateWaveButton();
            uiControl.UpdateGoldValue(currentGold);
            uiControl.UpdateHealthValue(levelHealth);
            GameCoordinator.Instance.currentGold = currentGold;

            log.Information($"Level {levelId} Instantiated.");
		}

        private void SetVars()
        {
            // Ping the game DB for Level meta data.
            LevelData levelData = GameDataBase.Instance.QueryLevelData((string)GetMeta("levelId"));
            levelHealth = levelData.levelHealth;
            currentGold = levelData.startGold;
            waves = levelData.waves;
            wavesToGo = waves.Keys.ToList();
        }

        public override void _Process(double delta)
        {
            TrackHealth();
            TrackWaveState();
            TrackGold();
        }

        private void TrackHealth()
        {
            // Checks if enemies have hit the end node and subtracts health if so.
            if(GameCoordinator.Instance.enemyBreach)
            {
                log.Information("Enemy has escaped, ticking away health.");
                int breachNum = GameCoordinator.Instance.breachNum;
                if (levelHealth <= breachNum)
                {
                    GD.Print("GAME OVER");
                }
                int health = levelHealth - breachNum;
                uiControl.UpdateHealthValue(health);
                GameCoordinator.Instance.enemyBreach = false;
            }
        }

        private void TrackWaveState()
        {
            // Checks if the wave has been cleared, if so spawns a button for the next wave.
            int currentActiveEnemies = GameCoordinator.Instance.activeEnemies.Count;
            if(waveActive && currentActiveEnemies == 0)
            {
                log.Information("Wave complete, Create new wave button.");
                waveActive = false;
                CreateWaveButton();
            }
        }

        private void TrackGold()
        {
            // Checks if we've bought a tower (removing gold) or killed an enemy (added gold)
            if(GameCoordinator.Instance.currentGold != currentGold)
            {
                log.Information("Gold amount has changed, update gold UI.");
                currentGold = GameCoordinator.Instance.currentGold;
                uiControl.UpdateGoldValue(currentGold);
            }
        }

        public void CreateWaveButton()
        {
            string name = $"Start Wave {currentWave}";
            waveButton = uiControl.CreateWaveButton(name);
            waveButton.Pressed += OnWaveButton;
        }

        private async Task SpawnWave()
        {
            waveActive = true;
            List<SpawnData> waveData = waves[currentWave];

            // This is an example of spawnData - {"name": "ghost", "multiplier": "3"}
            foreach(SpawnData spawnData in waveData)
            {
                int multiplier = int.Parse(spawnData.multiplier);
                for (int i= 1; i <= multiplier; i++)
                {
                    // Spawn enemy every second
                    SpawnEnemy(spawnData.enemyId);
                    await Task.Delay(1000);
                }
            }
        }

		public void SpawnEnemy(string enemyId)
		{
            // Spawn a single enemy to the level's path. First we need to grab the enemy data.
            EnemyData enemyData = GameDataBase.Instance.QueryEnemyData(enemyId);
			PackedScene prefab = GD.Load<PackedScene>($"{GameCoordinator.Instance.enemyPrefabLoc}/{enemyData.prefab}");
			PathFollow2D enemyPathFollow = (PathFollow2D) prefab.Instantiate();
            levelPath.AddChild(enemyPathFollow);

            // This should maybe be changed in the future.  Currently The parent node isn't holding the script because its
            // a pathfollow2D node and not a characterbody2D node.  The character body is within the pathfollow2D node so
            // I have to grab it by name.
            Enemy enemy = enemyPathFollow.GetNode<Enemy>(enemyData.name);

            // Pass data to the game coordinator
            GameCoordinator.Instance.activeEnemies.Add(enemy);
		}

        public async void OnWaveButton()
        {
            // Free up for garabage collection after deletion.
            waveButton.QueueFree();
            waveButton = null;

            await SpawnWave();
            wavesToGo.Remove(currentWave);
            currentWave = wavesToGo.Min();
        }

	}
}
