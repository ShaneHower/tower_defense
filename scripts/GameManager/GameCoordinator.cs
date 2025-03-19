namespace GameNamespace.GameManager
{
    using System.Collections.Generic;
    using GameNamespace.GameAssets;
    using GameNamespace.UI;

    using Godot;
    using Serilog;

    /// <summary>
    /// This singlton passes data between all of the scripts at run time.
    /// </summary>
    public partial class GameCoordinator : Node
    {
        public static GameCoordinator Instance { get; private set; }
        public DevWindow devWindow;
        public List<Enemy> activeEnemies = new();
        public bool enemyBreach = false;
        public int breachNum = 0;
        public int currentGold = 0;

        // Prefab locations
        public string prefabLoc = "res://prefabs";
        public string enemyPrefabLoc = "res://prefabs/enemies";
        public string projectilePrefabLoc = "res://prefabs/projectiles";
        public string towerPrefabLoc = "res://prefabs/towers";
        public string uiPrefabLoc = "res://prefabs/ui";
        private static readonly ILogger log = Log.ForContext<GameCoordinator>();

        /// <summary>
        /// This is a singleton class.  It would be bad if we had more than one, so we force duplicates
        /// to delete if there are any.
        /// </summary>
        public override void _Ready()
        {
            Instance = this;
            Logger.Init();
            log.Information("Game started.");
        }

        public override void _ExitTree()
        {
            log.Information($"[GameCoordinator] _ExitTree at {Time.GetTicksMsec()}");
            Logger.Shutdown();
        }
    }
}
