namespace GameNamespace.DataBase
{
    using Godot;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using Serilog;

    public partial class GameDataBase: Node
    {
        public static GameDataBase Instance { get; private set;}
        public string configLocation = "scripts/DataBase/Configs";
        private static readonly ILogger log = Log.ForContext<GameDataBase>();
        public Dictionary<string, LevelData> levelData;
        public Dictionary<string, EnemyData> enemyData;
        public Dictionary<string, TowerData> towerData;
        public Dictionary<string, ProjectileData> projectileData;

        public override void _Ready()
        {
            Instance = this;
            LoadConfigs();
            log.Information("Database Instantiated.");
        }

        public void LoadConfigs()
        {
            // Parse and unload enemy config
            string enemyJson = File.ReadAllText($"{configLocation}/Enemies/config.json");
            enemyData = JsonSerializer.Deserialize<Dictionary<string, EnemyData>>(enemyJson);

            // Parse and unload tower config
            string towerJson = File.ReadAllText($"{configLocation}/Towers/config.json");
            towerData = JsonSerializer.Deserialize<Dictionary<string, TowerData>>(towerJson);

            // Parse and unload projectile config
            string projectileJson = File.ReadAllText($"{configLocation}/Projectiles/config.json");
            projectileData = JsonSerializer.Deserialize<Dictionary<string, ProjectileData>>(projectileJson);

            // Parse and unload level config
            string levelJson = File.ReadAllText($"{configLocation}/Levels/config.json");
            levelData = JsonSerializer.Deserialize<Dictionary<string, LevelData>>(levelJson);
        }

        public EnemyData QueryEnemyData(string id)
        {
            log.Information($"Query Enemy Data for id: {id}");
            if(enemyData != null && enemyData.TryGetValue(id, out EnemyData data))
            {
                return data;
            }
            else
            {
                return null;
            }
        }

        public TowerData QueryTowerData(string id)
        {
            log.Information($"Query Tower Data for id: {id}");
            if(towerData != null && towerData.TryGetValue(id, out TowerData data))
            {
                return data;
            }
            else
            {
                return null;
            }
        }

        public ProjectileData QueryProjectileData(string id)
        {
            log.Information($"Query Projectile Data for id: {id}");
            if(projectileData != null && projectileData.TryGetValue(id, out ProjectileData data))
            {
                return data;
            }
            else
            {
                return null;
            }
        }

        public LevelData QueryLevelData(string id)
        {
            log.Information($"Query Level Data for id: {id}");
            if(levelData != null && levelData.TryGetValue(id, out LevelData data))
            {
                return data;
            }
            {
                return null;
            }
        }
    }

}
