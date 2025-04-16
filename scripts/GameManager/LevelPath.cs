namespace GameNamespace.GameManager
{
    using Godot;
    using System.Collections.Generic;
    using GameNamespace.DataBase;
    using GameNamespace.GameAssets;

    /// <summary>
    /// The PathIndex object is responsible for signalling to the enemies when they need to change animations. They are
    /// little colliders which are placed on each indice of the enemy path.  Once an enemy collides with one of these
    /// indices, this object will send a message to the enemy signalling that a new animation direction should be
    /// triggered.
    /// We also use this object to know when an enemy has hit the end of the level, thus removing health from the player.
    /// </summary>
    public partial class LevelPath : Area2D
    {
        // Class vars
        public Dictionary<int, PathIndex> pathIndices = new();
        public CollisionPolygon2D collider;
        public Node2D spawn;

        public override void _Ready()
        {
            collider = GetNode<CollisionPolygon2D>("Collider");
            spawn = GetNode<Node2D>("Spawn");

            Connect("area_entered", new Callable(this, nameof(OnAreaEnter)));
            Connect("area_exited", new Callable(this, nameof(OnAreaExit)));
        }

        public void SpawnEnemy(string enemyId, EnemyData passedData=null)
		{
            EnemyData enemyData = passedData ?? GameDataBase.Instance.QueryEnemyData(enemyId);

            // Spawn a single enemy to the level's path. First we need to grab the enemy data.
			PackedScene prefab = GD.Load<PackedScene>($"{GameCoordinator.Instance.enemyPrefabLoc}/{enemyData.prefab}");
			Enemy enemy = (Enemy)prefab.Instantiate();
            enemy.passedData = enemyData;
            spawn.AddChild(enemy);

            // Pass data to the game coordinator
            GameCoordinator.Instance.activeEnemies.Add(enemy);
		}

        public void OnAreaEnter(Area2D area)
        {
            if (area.Name == "PlacementArea" && area.GetParent() is Tower tower)
            {
                tower.canPlace = false;
                tower.badArea = true;
                tower.Modulate = new Color(1, 0.4f, 0.4f, 0.7f);
            }
        }

        public void OnAreaExit(Area2D area)
        {
            if (area.Name == "PlacementArea" && area.GetParent() is Tower tower)
            {
                tower.canPlace = true;
                tower.badArea = false;
                tower.Modulate = new Color(1, 1, 1, 1);
            }
        }

    }
}
