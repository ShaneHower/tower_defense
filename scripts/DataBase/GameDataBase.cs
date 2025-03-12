using Godot;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace GameNamespace.DataBase
{
    public partial class GameDataBase: Node
    {
        public static GameDataBase Instance { get; private set;}
        public string configLocation = "scripts/DataBase/configs";
        public Dictionary<string, LevelData> levelData;
        public Dictionary<string, EnemyData> enemyData;
        public Dictionary<string, TowerData> towerData;

        public override void _Ready()
        {
            if(Instance != null)
            {
                QueueFree();
                return;
            }

            Instance = this;
            LoadConfigs();
        }

        public void LoadConfigs()
        {
            // Parse and unload enemy config
            string enemyJson = File.ReadAllText($"{configLocation}/Enemies/config.json");
            enemyData = JsonSerializer.Deserialize<Dictionary<string, EnemyData>>(enemyJson);

            // Parse and unload tower config
            string towerJson = File.ReadAllText($"{configLocation}/Towers/config.json");
            towerData = JsonSerializer.Deserialize<Dictionary<string, TowerData>>(towerJson);

            // Parse and unload level config
            string levelJson = File.ReadAllText($"{configLocation}/Levels/config.json");
            levelData = JsonSerializer.Deserialize<Dictionary<string, LevelData>>(levelJson);
        }

        public EnemyData QueryEnemyData(string id)
        {
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
            if(towerData != null && towerData.TryGetValue(id, out TowerData data))
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
