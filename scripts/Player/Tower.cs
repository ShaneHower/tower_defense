namespace GameNamespace.Player
{
	using Godot;
	using System.Collections.Generic;
	using GameNamespace.Enemies;
	using GameNamespace.GameManager;
    using GameNamespace.DataBase;

    /// <summary>
    /// Basic tower class. Holds high level behavior and meta data about all towers.
    /// </summary>
    public partial class Tower : Area2D
	{
		// Class vars
		public string id;
		public string name;
		public float attackSpeed;
		public int gold;
		public float radius;
		public string prefab;
		public string projectilePrefab;
		public bool canFire = true;
		public bool beingPlaced = false;
		private List<Enemy> targetEnemies = new();
		private int targetOrder = 0;

		// Game objects
		public PackedScene projectile;
		public Projectile proj_instance;
		public AnimatedSprite2D animator;
		private Line2D towerRange;

		public override void _Ready()
		{
			// Set class vars
			id = (string)GetMeta("towerId");
			SetVars();

			// init work
			animator = GetNode<AnimatedSprite2D>("Animator");
			animator.Play("idle");
			towerRange = BuildTowerRange();
		}

		public void SetVars()
		{
			// Ping the game DB for Enemy meta data.
			TowerData data = GameDataBase.Instance.QueryTowerData(id);
			attackSpeed = data.attackSpeed;
			gold = data.gold;
			radius = data.radius;
			name = data.name;
			prefab = data.prefab;

			// Get the projectile prefab
			ProjectileData projectileData = GameDataBase.Instance.QueryProjectileData(data.projectileId);
			projectilePrefab = projectileData.prefab;
		}

		public override void _Process(double delta)
		{
			if(beingPlaced)
			{
				towerRange.Visible = true;
			}
			else
			{
				towerRange.Visible = false;
				AttackTarget();
			}
		}

        /// <summary>
		/// This method creates a visible circle representing the tower range.  This is visible when the tower is being
		/// placed.
		/// </summary>
		/// <returns></returns>
		private Line2D BuildTowerRange()
		{
			CollisionShape2D collider = GetNode<CollisionShape2D>("Collider");
			CircleShape2D circleCollider = (CircleShape2D)collider.Shape;

			// the higher the value of points the smoother the circle
			float scale = collider.Scale.X;
			float radius = circleCollider.Radius * scale;
			int points = 60;

			Line2D circleLine = new();
			circleLine.Width = 1;
			circleLine.DefaultColor = new Color(1, 0, 0, 1);

			Vector2[] circlePoints = new Vector2[points + 1];
			for(int i = 0; i <= points; i++)
			{
				// Mathf.Tau = 2 * PI
				float angle = (i / (float)points) * Mathf.Tau;
				circlePoints[i] = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
			}

			circleLine.Points = circlePoints;
			AddChild(circleLine);

			return circleLine;
		}

		private async void AttackTarget()
		{
			if(targetEnemies.Count > 0)
			{
				// Get first element of the list
				Enemy target = targetEnemies[0];

				// Sometimes an enemy may have been killed by another tower in this case the tower can get stuck
				// looking for an enemy that no longer exists.
				List<Enemy> activeEnemies = GameCoordinator.Instance.activeEnemies;
				if(activeEnemies.Contains(target))
				{
					if(canFire && !target.isDying)
					{
						canFire = false;

						// Instantiate projectile
						projectile = GD.Load<PackedScene>($"{GameCoordinator.Instance.projectilePrefabLoc}/{projectilePrefab}");
						proj_instance = (Projectile) projectile.Instantiate();
						AddChild(proj_instance);
						proj_instance.target = target;

						// Wait out the attack speed
						await ToSignal(GetTree().CreateTimer(attackSpeed), "timeout");
						canFire = true;
					}
				}
				else
				{
					targetEnemies.Remove(target);
				}
			}
		}

		private void OnEnter(Enemy enemy)
		{
			enemy.targeted = true;
			targetEnemies.Add(enemy);
		}

		// Called when another body exits the area
		private void OnExit(Enemy enemy)
		{
			// For now I'm treating this like a stack, first in first out.
			enemy.targeted = false;
			targetEnemies.Remove(enemy);
		}

	}
}
