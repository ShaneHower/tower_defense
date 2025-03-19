namespace  GameNamespace.GameAssets
{
	using Godot;
	using System.Collections.Generic;
	using GameNamespace.GameManager;
    using GameNamespace.DataBase;
	using Serilog;

    /// <summary>
    /// Basic tower class. Holds high level behavior and meta data about all towers.
    /// </summary>
    public partial class Tower : Area2D
	{
		// Data
		public string id;
		public string type;
		public string name;
		public int level;
		public float radius;
		public int gold;
		public float attackSpeed;
		public string prefab;
		public string projectileId;
		public string attackModifier;
		public int attackModCounter;
		public string nextLevelId;

		// Class vars
		public int attackCounter = 1;
		public bool canFire = true;
		public bool beingPlaced = false;
		private List<Enemy> targetEnemies = new();
		private int targetOrder = 0;
		public bool isHovered = false;
		private static readonly ILogger log = Log.ForContext<Tower>();

		// Game objects
		public PackedScene projectile;
		public Projectile proj_instance;
		public AnimatedSprite2D animator;
		private Line2D towerRange;
		private Area2D hoverArea;
		private Control upgradeControl;
		private Button upgradeButton;
		private Level gameLevel;

		public override void _Ready()
		{
			// Set class vars
			id = (string)GetMeta("towerId");
			SetVars();

			// Init work
			gameLevel = GetTree().Root.GetNode<Level>("Level");
			animator = GetNode<AnimatedSprite2D>("Animator");
			animator.Play("idle");

			upgradeControl = GetNode<Control>("UpgradeControl");
			upgradeButton = upgradeControl.GetNode<Button>("Upgrade");
			upgradeButton.Pressed += Upgrade;


			hoverArea = GetNode<Area2D>("HoverArea");
			hoverArea.MouseEntered += OnMouseHover;
			hoverArea.MouseExited += OnMouseLeave;

			towerRange = BuildTowerRange();

			log.Information($"Tower {this} with name {this.Name} instantiated.");
		}

		public void SetVars()
		{
			// Ping the game DB for Enemy meta data.
			TowerData data = GameDataBase.Instance.QueryTowerData(id);
			type = data.type;
			name = data.name;
			level = data.level;
			radius = data.radius;
			gold = data.gold;
			attackSpeed = data.attackSpeed;
			prefab = data.prefab;
			projectileId = data.projectileId;
			attackModifier = data.attackModifier;
			attackModCounter = data.attackModCounter;
			nextLevelId = data.nextLevelId;
		}

		public override void _Process(double delta)
		{
			if(beingPlaced)
			{
				towerRange.Visible = true;
				hoverArea.Visible = false;
			}
			else
			{
				towerRange.Visible = false;
				hoverArea.Visible = true;

				if(attackCounter * attackModCounter != 0 && attackCounter % attackModCounter == 0)
				{

					AttackTarget(attackModifier);
				}
				else
				{
					AttackTarget(projectileId);
				}
			}
		}

		public override void _Input(InputEvent @event)
        {
            if(@event is InputEventMouseButton mouseEvent)
			{
				if(isHovered && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
				{
					upgradeControl.Visible = true;
				}
				if(mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Right)
				{
					if(upgradeControl.Visible)
					{
						upgradeControl.Visible = false;
					}
				}
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

		private async void AttackTarget(string projectileId)
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
						log.Information($"Tower {this} with name {this.Name} is attacking Enemy {target}");
						canFire = false;

						// Instantiate projectile
						// Get the projectile prefab
						ProjectileData projectileData = GameDataBase.Instance.QueryProjectileData(projectileId);
						string projectilePrefab = projectileData.prefab;
						projectile = GD.Load<PackedScene>($"{GameCoordinator.Instance.projectilePrefabLoc}/{projectilePrefab}");
						proj_instance = (Projectile) projectile.Instantiate();
						AddChild(proj_instance);
						proj_instance.target = target;

						// Wait out the attack speed
						await ToSignal(GetTree().CreateTimer(attackSpeed), "timeout");
						attackCounter++;
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

		private void OnMouseHover()
		{
			isHovered = true;
		}

		private void OnMouseLeave()
		{
			isHovered = false;
		}

		private void Upgrade()
		{
			log.Information($"Tower {this} with name {this.Name} is attempting to upgrade");
			TowerData data = GameDataBase.Instance.QueryTowerData(nextLevelId);
			if(GameCoordinator.Instance.currentGold > data.gold)
			{
				// Generate the tower prefab.
				PackedScene prefab = GD.Load<PackedScene>($"{GameCoordinator.Instance.towerPrefabLoc}/{data.prefab}");
				Tower upgrade = (Tower)prefab.Instantiate();
				gameLevel.AddChild(upgrade);
				upgrade.Position = Position;
				GameCoordinator.Instance.currentGold -= upgrade.gold;
				QueueFree();
			}
		}

	}
}
